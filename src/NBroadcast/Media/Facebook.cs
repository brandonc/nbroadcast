using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace NBroadcast.Media
{
    public class Facebook : Medium<Facebook>, IMedium
    {
        public static void Setup(Setup setup)
        {
            setup.ValidateExists("appid", "token", "page");
            Medium<Facebook>.setup = setup;
        }

        public void Dispatch(string body)
        {
            //FacebookOAuthClient oauth = new FacebookOAuthClient();
            //oauth.AppId = GetVal("appid");
            //oauth.AppSecret = GetVal("appsecret");

            //var parameters = new Dictionary<string, object> {
            //    { "response_type", "code" },
            //    { "display", "page" },
            //    { "scope", "offline_access, publish_stream" }
            //};

            //dynamic token = oauth.GetApplicationAccessToken(parameters);

            //FacebookClient client = new FacebookClient(
            //    token["access_token"]
            //);

            //dynamic response = client.Post(GetVal("user") + "/feed", new Dictionary<string, object> { { "message", body }});
        }
    }
}
