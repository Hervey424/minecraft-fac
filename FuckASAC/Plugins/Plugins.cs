using FuckASAC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FuckASAC.Plugins
{
    public static class Plugins
    {
        private static List<IPlugin> plugins;

        static Plugins()
        {
            plugins = new List<IPlugin>();
            plugins.Add(new ASACPlugin());
        }

        public static bool STCHandle(Package package, BinaryWriter write)
        {
            foreach(IPlugin plugin in plugins)
            {
                bool result = plugin.STCHandle(package, write);
                if(result)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CTSHandle(Package package, BinaryWriter write)
        {
            foreach (IPlugin plugin in plugins)
            {
                bool result = plugin.CTSHandle(package, write);
                if (result)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
