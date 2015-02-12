using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFramework
{
    /// <summary>
    /// Marks a class to be loaded by the LoadTypesWithBase method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class AutoLoadAttribute : Attribute
    {
    }
}
