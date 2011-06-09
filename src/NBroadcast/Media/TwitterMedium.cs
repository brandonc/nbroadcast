using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twitterizer;

namespace NBroadcast.Media
{
    public class TwitterMedium : Medium<TwitterMedium>, IMedium
    {
        public static void Setup(Setup setup)
        {
            Medium<TwitterMedium>.setup = setup;
            setup.ValidateExists("accesstoken", "accesstokensecret");
        }

        public void Dispatch(string body)
        {
            var tokens = new Twitterizer.OAuthTokens();
            tokens.AccessToken = GetVal("accesstoken");
            tokens.AccessTokenSecret = GetVal("accesstokensecret");
            tokens.ConsumerKey = GetVal("consumerkey");
            tokens.ConsumerSecret = GetVal("consumersecret");

            TwitterResponse<TwitterStatus> resp = TwitterStatus.Update(tokens, body);
            if (resp.Result != RequestResult.Success)
            {
                throw new NoticeDispatchException("Could not dispatch notice to twitter. The error returned was: " + resp.Result.ToString());
            }
        }
    }
}
