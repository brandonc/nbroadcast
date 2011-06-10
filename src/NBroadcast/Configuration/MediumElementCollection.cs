using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NBroadcast.Configuration
{
    public class MediumElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {   
            return new MediumElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MediumElement)element).Name;
        }
    }
}
