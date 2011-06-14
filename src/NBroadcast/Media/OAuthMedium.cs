using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using NBroadcast.OAuth;
using System.IO;

namespace NBroadcast.Media
{
    public abstract class OAuthMedium<T> : Medium<T>
    {
        protected string apiroot;
        protected bool usehttps = false;

        protected class OAuthEndpoints
        {
            public string RequestTokenEndpoint { get; set; }
            public string AuthorizeEndpoint { get; set; }
            public string AccessTokenEndpoint { get; set; }
        }

        public class OAuthTokens
        {
            public string ConsumerKey { get; set; }
            public string ConsumerSecret { get; set; }
            public string AccessToken { get; set; }
            public string AccessTokenSecret { get; set; }
        }

        public class AuthRequestToken
        {
            public string RequestToken { get; set; }
            public string RequestTokenSecret { get; set; }
            public Uri AuthorizeUrl { get; set; }
        }

        protected abstract OAuthEndpoints Endpoints { get; }

        public OAuthTokens Tokens
        {
            get
            {
                return new OAuthTokens() {
                    ConsumerKey = GetValOrBlank("consumerkey"),
                    ConsumerSecret = GetValOrBlank("consumersecret"),  
                    AccessToken = GetValOrBlank("accesstoken"),
                    AccessTokenSecret = GetValOrBlank("accesstokensecret")
                };
            }
        }

        private static WebRequest CreateSignedRequest(OAuthTokens tokens, string method, string url, Dictionary<string, string> param, string callbackurl, string verifier) {
            OAuthBase oAuth = new OAuthBase();
            
            StringBuilder sb = new StringBuilder();
            if (param != null)
            {
                foreach (var pair in param)
                {
                    if (sb.Length > 0)
                        sb.Append("&");

                    sb.AppendFormat("{0}={1}", oAuth.UrlEncode(pair.Key), oAuth.UrlEncode(pair.Value));
                }
            }

            Uri uri = new Uri(url + (sb.Length == 0 ? "" : "?" + sb.ToString()));
            string url_out;
            string params_out;
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string sig = oAuth.GenerateSignature(uri,
                tokens.ConsumerKey, tokens.ConsumerSecret,
                tokens.AccessToken, tokens.AccessTokenSecret,
                method, timeStamp, nonce, callbackurl, verifier,
                OAuthBase.SignatureTypes.HMACSHA1, out url_out, out params_out);

            sig = HttpUtility.UrlEncode(sig);

            var request = HttpWebRequest.Create(uri);
            request.Method = method;

            string authorization = String.Format("OAuth oauth_nonce=\"{0}\", {1}oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"{2}\", {3}oauth_signature=\"{4}\", {5}oauth_version=\"1.0\"{6}",
                nonce,
                (String.IsNullOrEmpty(callbackurl) ? "" : String.Format("oauth_callback=\"{0}\", ", oAuth.UrlEncode(callbackurl))),
                timeStamp,
                (String.IsNullOrEmpty(tokens.ConsumerKey) ? "" : String.Format("oauth_consumer_key=\"{0}\", ", tokens.ConsumerKey)),
                sig,
                (String.IsNullOrEmpty(verifier) ? "" : String.Format("oauth_verifier=\"{0}\", ", verifier)),
                (String.IsNullOrEmpty(tokens.AccessToken) ? "" : String.Format(", oauth_token=\"{0}\"", tokens.AccessToken))
            );

            request.Headers.Add(HttpRequestHeader.Authorization, authorization);

            return request;
        }

        public AuthRequestToken Authorize(string consumerkey, string consumersecret)
        {
            if (consumerkey == null)
                throw new ArgumentNullException("consumerkey");

            if (consumersecret == null)
                throw new ArgumentNullException("consumersecret");

            var tokens = new OAuthTokens()
            {
                ConsumerKey = consumerkey,
                ConsumerSecret = consumersecret
            };

            WebRequest req = CreateSignedRequest(tokens, "POST", Endpoints.RequestTokenEndpoint, null, "oob", null);
            using (var response = req.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var qs = HttpUtility.ParseQueryString(reader.ReadToEnd());
                    return new AuthRequestToken() {
                        AuthorizeUrl = new Uri(Endpoints.AuthorizeEndpoint + "?oauth_token=" + qs["oauth_token"]),
                        RequestToken = qs["oauth_token"],
                        RequestTokenSecret = qs["oauth_token_secret"]
                    };
                }
            }
        }

        public OAuthTokens GetAccessToken(string consumerkey, string consumersecret, AuthRequestToken reqtoken, string verifier)
        {
            if (consumerkey == null)
                throw new ArgumentNullException("consumerkey");

            if (consumersecret == null)
                throw new ArgumentNullException("consumersecret");

            if (reqtoken == null)
                throw new ArgumentNullException("reqtoken");

            if (verifier == null)
                throw new ArgumentNullException("verifier");

            var tokens = new OAuthTokens()
            {
                ConsumerKey = consumerkey,
                ConsumerSecret = consumersecret,
                AccessToken = reqtoken.RequestToken,
                AccessTokenSecret = reqtoken.RequestTokenSecret
            };

            WebRequest req = CreateSignedRequest(tokens, "POST", Endpoints.AccessTokenEndpoint, null, null, verifier);
            using (var response = req.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var qs = HttpUtility.ParseQueryString(reader.ReadToEnd());
                    return new OAuthTokens()
                    {
                        ConsumerKey = consumerkey,
                        ConsumerSecret = consumersecret,
                        AccessToken = qs["oauth_token"],
                        AccessTokenSecret = qs["oauth_token_secret"]
                    };
                }
            }
        }

        protected WebRequest Post(string path, Dictionary<string, string> parameters)
        {
            return CreateSignedRequest(this.Tokens, "POST", (usehttps ? "https://" : "http://") + apiroot + path, parameters, null, null);
        }

        protected WebRequest Get(string path, Dictionary<string, string> parameters)
        {
            return CreateSignedRequest(this.Tokens, "GET", (usehttps ? "https://" : "http://") + apiroot + path, parameters, null, null);
        }

        static OAuthMedium()
        {
            System.Net.ServicePointManager.Expect100Continue = false;
        }
    }
}
