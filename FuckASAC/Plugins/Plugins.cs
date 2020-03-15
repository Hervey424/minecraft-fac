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
            // 必要插件
            plugins.Add(new ModListPlugin());
            // ASAC
            plugins.Add(new ASACPlugin());
            // CatAntiCheat
            plugins.Add(new CatAntiCheatPlugin());

        }

        public static bool STCHandle(Package package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            foreach(IPlugin plugin in plugins)
            {
                bool result = plugin.STCHandle(package, toClientWriter, toServerWriter);
                if(result)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CTSHandle(Package package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            foreach (IPlugin plugin in plugins)
            {
                bool result = plugin.CTSHandle(package, toClientWriter, toServerWriter);
                if (result)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
