using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace PluginFramework
{
    /// <summary>
    /// Loads and manages plugins.
    /// </summary>
    public class PluginLoader : IDisposable
    {
        private readonly ConcurrentDictionary<Type, bool> _loadedTypes = new ConcurrentDictionary<Type, bool>();
        private readonly Dictionary<Type, PluginBase> _parts = new Dictionary<Type, PluginBase>();

        /// <summary>
        /// Adds classes with the AutoLoadAttribute found a given assembly to the loaded classes list.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void AddAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract) continue;

                if (Attribute.IsDefined(type, typeof(AutoLoadAttribute), true))
                {
                    var constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor != null)
                        this._loadedTypes.TryAdd(type, true);
                }
            }
        }

        /// <summary>
        /// Enables the given class.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <returns></returns>
        public T Enable<T>() where T : PluginBase, new()
        {
            return this.Enable<T>(null);
        }

        /// <summary>
        /// Enables the given class and runs the preconstructor provided.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <param name="preConstructor">The preconstructor.</param>
        /// <returns></returns>
        public T Enable<T>(Action<T> preConstructor) where T : PluginBase, new()
        {
            return (T)this.EnableInternal(typeof(T), o => preConstructor((T)o));
        }

        /// <summary>
        /// Loads the given class.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        public T Load<T>() where T : PluginBase, new()
        {
            return this.Load<T>(null);
        }

        /// <summary>
        /// Loads the given class and runs the preconstructor provided.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <param name="preConstructor">The preconstructor.</param>
        public T Load<T>(Action<T> preConstructor) where T : PluginBase, new()
        {
            return (T)this.LoadInternal(typeof(T), o => preConstructor((T)o));
        }

        /// <summary>
        /// Loads the types with the specified base class.
        /// </summary>
        /// <typeparam name="T">The base class.</typeparam>
        public void LoadTypesWithBase<T>() where T : PluginBase
        {
            this.LoadTypesWithBase<T>(null);
        }

        /// <summary>
        /// Loads the types with the specified base class.
        /// </summary>
        /// <typeparam name="T">The base class.</typeparam>
        /// <param name="preConstructor">The preconstructor.</param>
        public void LoadTypesWithBase<T>(Action<T> preConstructor) where T : PluginBase
        {
            foreach (var type in _loadedTypes.Keys)
            {
                if (typeof(T).IsAssignableFrom(type))
                {
                    this.LoadInternal(type, o => preConstructor((T)o));
                }
            }
        }

        private PluginBase LoadInternal(Type type, Action<object> preConstructor)
        {
            if (this._parts.ContainsKey(type))
                throw new InvalidOperationException("The given type has already been loaded.");

            var obj = this.EnableInternal(type, preConstructor);

            this._parts.Add(type, obj);
            bool val;
            this._loadedTypes.TryRemove(type, out val);

            return obj;
        }

        private PluginBase EnableInternal(Type type, Action<object> preConstructor)
        {
            var obj = (PluginBase)FormatterServices.GetUninitializedObject(type);
            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new InvalidOperationException(
                    "the given type doesn't contain a default public parameterless constructor.");

            obj.Loader = this;
            if (preConstructor != null)
                preConstructor(obj);
            constructor.Invoke(obj, null);

            return obj;
        }

        internal void UnloadInternal(Type type)
        {
            this._parts.Remove(type);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposed)
        {
            if (disposed)
            {
                this._loadedTypes.Clear();

                PluginBase[] parts;
                parts = this._parts.Values.ToArray();

                foreach (var part in parts)
                {
                    ((IDisposable)part).Dispose();
                }
            }
        }
    }
}