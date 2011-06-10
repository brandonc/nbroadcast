using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NBroadcast
{
    static class KeyValueConfigurationCollectionExtensions
    {
        public static Setup ToSetup(this KeyValueConfigurationCollection config)
        {
            var result = new Setup();
            foreach (KeyValueConfigurationElement el in config)
            {
                result.Add(el.Key, el.Value);
            }
            return result;
        }
    }
}
