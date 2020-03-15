using FuckASAC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FuckASAC.Plugins
{
    public abstract class AbsolutePlugin : PluginMessageHandle
    {
        public override bool CTSPluginMessageHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            return false;
        }

        public override bool STCPluginMessageHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            return false;
        }
    }
}
