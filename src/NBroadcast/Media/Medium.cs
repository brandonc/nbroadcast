using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;

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
            if (!HasVal(key) || GetVal(key) == null)
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

        protected bool HasVal(string key)
        {
            return (setup != null && setup.ContainsKey(key) && setup[key] != null);
        }

        protected virtual void HandleWebExceptions(WebException ex)
        {
            HttpWebResponse resp = (HttpWebResponse)ex.Response;

            if (resp == null)
                throw new NoticeDispatchException("The server did not respond. Please try again later.");

            switch (resp.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    throw new NoticeDispatchException("The server returned an authorization error response. Double check your authorization credentials.");
                default:
                    throw new NoticeDispatchException(String.Format("The server returned an error. {0}: {1}", resp.StatusCode, resp.StatusDescription));
            }
        }
    }
}
