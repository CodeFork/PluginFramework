using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace PluginFramework
{
    public class PluginLoader : IDisposable
    {
        private readonly ConcurrentDictionary<Type, bool> _loadedTypes = new ConcurrentDictionary<Type, bool>();
        private readonly ConcurrentDictionary<Type, PluginBase> _parts = new ConcurrentDictionary<Type, PluginBase>();
        
        public void LoadAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract) continue;

                if (Attribute.IsDefined(type, typeof(AutoLoadAttribute), true))
                {
                    this._loadedTypes.TryAdd(type, true);
                }
            }
        }

        public void Load<T>() where T : PluginBase, new()
        {
            this.Load<T>(null);
        }

        public void Load<T>(Action<T> preConstructor) where T : PluginBase, new()
        {
            this.LoadInternal(typeof(T), o => preConstructor((T)o));
        }

        public void LoadTypesWithBase<T>() where T : PluginBase
        {
            this.LoadTypesWithBase<T>(null);
        }

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

        private void LoadInternal(Type type, Action<object> preConstructor)
        {
            var obj = (PluginBase)FormatterServices.GetUninitializedObject(type);

            obj.Loader = this;
            if (preConstructor != null)
                preConstructor(obj);

            var constructor = type.GetConstructor(Type.EmptyTypes);
            Debug.Assert(constructor != null, "constructor != null");
            constructor.Invoke(obj, null);

            this._parts.TryAdd(type, obj);

            bool val;
            this._loadedTypes.TryRemove(type, out val);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposed)
        {
            this._loadedTypes.Clear();
            foreach (var part in this._parts)
            {
                ((IDisposable)part.Value).Dispose();
            }
        }
    }
}