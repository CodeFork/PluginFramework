using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFramework
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class AutoLoadAttribute : Attribute
    {
    }
}
