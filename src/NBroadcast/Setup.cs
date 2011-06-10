using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace NBroadcast
{
    public class Setup : Dictionary<string, object>
    {
        private Func<string, string> makeInvalidFormatMsg = 
            (key) => "Setup key \"" + key + "\" is not in the correct format.";

        private Func<string, string> makeOutOfRangeMsg =
            (key) => "Setup key \"" + key + "\" is out of range.";

        private Func<string[], string> makeRequiredMsg =
            (keys) => "The following keys are required for this setup: " + String.Join<string>(", ", keys);

        public void Add(string property, Func<object> value)
        {
            base.Add(property, value());
        }

        internal void ValidateExists(params string[] keys)
        {
            if (!keys.All(p => this.ContainsKey(p) && this[p] != null))
                throw new ArgumentException(makeRequiredMsg(keys));
        }

        internal void Validate(string key, Func<object, bool> validation)
        {
            if (base.ContainsKey(key))
            {
                try
                {
                    if (!validation(base[key]))
                        throw new ArgumentException(makeInvalidFormatMsg(key));
                }
                catch
                {
                    throw new ArgumentException(makeInvalidFormatMsg(key));
                }
            }
        }

        internal void ValidateRegex(string key, string pattern)
        {
            if (base.ContainsKey(key))
            {
                var regex = new Regex(pattern);
                if (!regex.IsMatch(base[key].ToString()))
                    throw new ArgumentException(makeInvalidFormatMsg(key));
            }
        }

        internal void ValidateRange(string key, int lower, int upper)
        {
            if (base.ContainsKey(key))
            {
                int garbage;
                if (!Int32.TryParse(base[key].ToString(), out garbage))
                    throw new ArgumentException(makeInvalidFormatMsg(key));

                if (garbage < lower || garbage > upper)
                {
                    throw new ArgumentOutOfRangeException(makeOutOfRangeMsg(key));
                }
            }
        }
    }
}
