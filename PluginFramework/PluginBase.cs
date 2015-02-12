using System;

namespace PluginFramework
{
    /// <summary>
    /// The base plugin class, all plugins must inherit from this class.
    /// </summary>
    public abstract class PluginBase : IDisposable
    {
        internal PluginLoader Loader { get; set; }

        void IDisposable.Dispose()
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
                this.Loader.UnloadInternal(this.GetType());
            }
        }
    }
}
