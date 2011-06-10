using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NBroadcast.Configuration
{
    public class MediumElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("", IsDefaultCollection = true, IsRequired=true)]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection))]
        public KeyValueConfigurationCollection Config
        {
            get
            {
                return (KeyValueConfigurationCollection)this[""];
            }
        }
    }
}
