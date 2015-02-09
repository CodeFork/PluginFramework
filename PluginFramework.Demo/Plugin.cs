using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFramework.Demo
{
    [AutoLoad]
    public abstract class Plugin : PluginBase
    {
        public int Id { get; set; }
    }
}
