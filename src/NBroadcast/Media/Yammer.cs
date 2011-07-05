using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace NBroadcast.Media
{
    public class Yammer : OAuthMedium<Yammer>, IMedium
    {
        public static void Setup(Setup setup)
        {
            Medium<Yammer>.setup = setup;
            setup.ValidateExists("consumerkey", "consumersecret", "accesstoken", "accesstokensecret");
        }

        protected override OAuthMedium<Yammer>.OAuthEndpoints Endpoints
        {
            get
            {
                return new OAuthEndpoints()
                {
                    RequestTokenEndpoint = "https://www.yammer.com/oauth/request_token",
                    AccessTokenEndpoint = "https://www.yammer.com/oauth/access_token",
                    AuthorizeEndpoint = "https://www.yammer.com/oauth/authorize"
                };
            }
        }

        public void Dispatch(string body)
        {
            try
            {
                var req = base.Post("/api/v1/messages.json", new Dictionary<string, string>() { { "body", body } });
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

        public Yammer()
        {
            usehttps = true;
            apiroot = "www.yammer.com";
        }
    }
}
