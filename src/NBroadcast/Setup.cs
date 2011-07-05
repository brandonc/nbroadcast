using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Configuration;
using NBroadcast.Configuration;
using NBroadcast.Media;
using System.Reflection;

namespace NBroadcast
{
    public class Setup : Dictionary<string, object>
    {
        // Simple exception message building delegates
        private Func<string, string> makeInvalidFormatMsg = 
            (key) => "Setup key \"" + key + "\" is not in the correct format.";

        private Func<string, string> makeOutOfRangeMsg =
            (key) => "Setup key \"" + key + "\" is out of range.";

        private Func<string[], string> makeRequiredMsg =
            (keys) => "The following keys are required for this setup: " + String.Join<string>(", ", keys);

        // This method must be called manually if `Setup` class is never instanced.
        public static void AutoConfig()
        {
            var section = (NBroadcastMediaConfigurationSection)ConfigurationManager.GetSection("nbroadcast");

            if (section != null)
            {

                foreach (MediumElement medium in section.Media)
                {
                    // AutoConfig uses reflection to call the normal setup functions of the specified `Medium`.
                    // The optional method `AutoConfigValueHelper` can be added to a Medium class in order to convert string values.
                    // If that method doesn't exist, string is assumed.

                    var mediumtype = Type.GetType("NBroadcast.Media." + medium.Name);
                    var helperMethod = mediumtype.GetMethod("AutoConfigValueHelper", BindingFlags.Static | BindingFlags.NonPublic);
                    var setupMethod = mediumtype.GetMethod("Setup", BindingFlags.Static | BindingFlags.Public);

                    var setup = new Setup();

                    foreach (KeyValueConfigurationElement el in medium.Config)
                    {
                        object value = el.Value;
                        if (helperMethod != null)
                            value = helperMethod.Invoke(null, new string[] { el.Key, el.Value });

                        setup.Add(el.Key, value);
                    }

                    try
                    {
                        setupMethod.Invoke(null, new object[] { setup });
                    } catch (TargetInvocationException ex)
                    {
                        // `ArgumentException` is swallowed here in case an incomplete configuration is given in XML
                        // but there is still an opportunity to use the `Setup()` routine.
                        if (ex.InnerException is ArgumentException)
                            continue;

                        throw;
                    }
                }
            }
        }

        // ### Setup validation

        // Simple 'required key' validation.
        internal void ValidateExists(params string[] keys)
        {
            if (!keys.All(p => this.ContainsKey(p) && this[p] != null))
                throw new ArgumentException(makeRequiredMsg(keys));
        }

        // Callback style validation
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

        // Regex style validation
        internal void ValidateRegex(string key, string pattern)
        {
            if (base.ContainsKey(key))
            {
                var regex = new Regex(pattern);
                if (!regex.IsMatch(base[key].ToString()))
                    throw new ArgumentException(makeInvalidFormatMsg(key));
            }
        }

        // Integer range validation
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

        // Before any manual setup takes place, try to load from app/web.config
        static Setup()
        {
            try
            {
                AutoConfig();
            } catch
            {
                // Ignore any exceptions because we weren't asked to do this.
            }
        }
    }
}
