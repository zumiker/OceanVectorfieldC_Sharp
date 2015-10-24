using System;
using System.Collections.Generic;

using Slb.Ocean.Core;

namespace VectorField
{
    public class VFPlugin : Slb.Ocean.Core.Plugin
    {
        public override string AppVersion
        {
            get { return "Unknown"; }
        }

        public override string Author
        {
            get { return "Novozhencev-Gorinov"; }
        }

        public override string Contact
        {
            get { return "contact@company.info"; }
        }

        public override IEnumerable<PluginIdentifier> Dependencies
        {
            get { return null; }
        }

        public override string Description
        {
            get { return ""; }
        }

        public override string ImageResourceName
        {
            get { return null; }
        }

        public override Uri PluginUri
        {
            get { return new Uri("http://www.pluginuri.info"); }
        }

        public override IEnumerable<ModuleReference> Modules
        {
            get 
            {
                // Please fill this method with your modules with lines like this:
                //yield return new ModuleReference(typeof(Module));
                yield return new ModuleReference(typeof(VFMod));

            }
        }

        public override string Name
        {
            get { return "VFPlugin"; }
        }

        public override PluginIdentifier PluginId
        {
            get { return new PluginIdentifier(typeof(VFPlugin).FullName, typeof(VFPlugin).Assembly.GetName().Version); }
        }

        public override ModuleTrust Trust
        {
            get { return new ModuleTrust("Default"); }
        }
    }
}
