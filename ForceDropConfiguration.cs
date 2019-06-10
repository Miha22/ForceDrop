#region Configuration
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ForceDrop
{
    public class ForceDropConfiguration : IRocketPluginConfiguration
    {
        public bool Enabled;
        [XmlArrayItem(ElementName = "ID")]
        public List<ushort> WhiteListedFromDrop;
        public void LoadDefaults()
        {
           WhiteListedFromDrop = new List<ushort>
            {
                116,
            };
            Enabled = true;
        }
    }
}
#endregion Configuration