using System;

namespace PluginFramework
{
    public abstract class PluginBase : IDisposable
    {
        protected internal PluginLoader Loader { get; set; }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposed)
        {
        }
    }
}
