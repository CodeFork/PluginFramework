using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFramework.Demo
{
    public class Plugin2 : Plugin
    {
        public Plugin2()
        {
            Console.WriteLine("Hi I'm Plugin2 my id is " + this.Id);
        }
    }
}
