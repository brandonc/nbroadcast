using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace NBroadcast.Media
{
    public class Twitter : OAuthMedium<Twitter>, IMedium
    {
        public static void Setup(Setup setup)
        {
            setup.ValidateExists("accesstoken", "accesstokensecret", "consumerkey", "consumersecret");
            Medium<Twitter>.setup = setup;
        }

        public void Dispatch(string body)
        {
            var req = base.Post("/1/statuses/update.json", new Dictionary<string, string>() { { "status", body } });
            using (WebResponse resp = req.GetResponse())
            {
                using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                {
                    string s = reader.ReadToEnd();
                }
            }
        }

        public Twitter()
        {
            base.apiroot = "api.twitter.com";
        }
    }
}
