using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFramework.Demo
{
    public class Plugin1 : Plugin
    {
        public Plugin1()
        {
            Console.WriteLine("Hi I'm Plugin1 my id is " + this.Id);
        }
    }
}
