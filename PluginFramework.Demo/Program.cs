using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PluginFramework.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var loader = new PluginLoader();
            loader.LoadAssembly(Assembly.GetExecutingAssembly());

            var id = 1;
            loader.LoadTypesWithBase<Plugin>(p =>
            {
                p.Id = id++;
            });
        }
    }
}
