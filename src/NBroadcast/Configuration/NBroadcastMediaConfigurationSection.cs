using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NBroadcast.Configuration
{
    class NBroadcastMediaConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection=true)]
        [ConfigurationCollection(typeof(MediumElementCollection), AddItemName="medium")]
        public MediumElementCollection Media
        {
            get
            {
                return (MediumElementCollection)this[""];
            }
        }
    }
}
