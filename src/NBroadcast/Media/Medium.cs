using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBroadcast.Media
{
    public abstract class Medium<T>
    {
        protected static Setup setup;

        // GetVal methods assumes setup has been validated

        protected string GetVal(string key)
        {
            EnsureSetup();

            try
            {
                return setup[key].ToString();
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        protected string GetValOrBlank(string key)
        {
            EnsureSetup();

            if (!HasSetup(key) || GetVal(key) == null)
                return String.Empty;

            return setup[key].ToString();
        }

        protected int GetValInt(string key)
        {
            EnsureSetup();

            return (int)setup[key];
        }

        protected bool GetValBool(string key)
        {
            EnsureSetup();

            return (bool)setup[key];
        }

        protected void EnsureSetup()
        {
            if (setup == null)
            {
                throw new MediumSetupException();
            }
        }

        protected bool HasSetup(string key)
        {
            return (setup.ContainsKey(key) && setup[key] != null);
        }
    }
}
