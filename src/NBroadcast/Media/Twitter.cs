using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace NBroadcast.Media
{
    public class Twitter : OAuthMedium<Twitter>, IMedium
    {
        public static void Setup(Setup setup)
        {
            Medium<Twitter>.setup = setup;
            setup.ValidateExists("accesstoken", "accesstokensecret", "consumerkey", "consumersecret");
        }

        public void Dispatch(string body)
        {
            var req = base.Post("/1/statuses/update.json", new Dictionary<string, string>() { { "status", body } });
            try
            {
                using (WebResponse resp = req.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        var json = new JavaScriptSerializer();
                        dynamic response = json.DeserializeObject(reader.ReadToEnd());
                    }
                }
            } catch (WebException ex)
            {
                HandleWebExceptions(ex);
            }
        }

        public Twitter()
        {
            base.apiroot = "api.twitter.com";
        }

        protected override OAuthMedium<Twitter>.OAuthEndpoints Endpoints
        {
            get {
                return new OAuthEndpoints()
                {
                    RequestTokenEndpoint = "https://api.twitter.com/oauth/request_token",
                    AccessTokenEndpoint = "https://api.twitter.com/oauth/access_token",
                    AuthorizeEndpoint = "https://api.twitter.com/oauth/authorize"
                };
            }
        }
    }
}
