using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using NBroadcast.OAuth;

namespace NBroadcast.Media
{
    public class OAuthMedium<T> : Medium<T>
    {
        protected string apiroot;

        private WebRequest GetRequest(string method, string path, Dictionary<string, string> param) {
            string url, normparams;
            OAuthBase oAuth = new OAuthBase();
            
            StringBuilder sb = new StringBuilder();
            foreach (var pair in param)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                sb.AppendFormat("{0}={1}", oAuth.UrlEncode(pair.Key), oAuth.UrlEncode(pair.Value));
            }

            Uri uri = new Uri("http://" + apiroot + path + "?" + sb.ToString());

            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string sig = oAuth.GenerateSignature(uri,
                GetVal("consumerkey"), GetVal("consumersecret"),  
                GetVal("accesstoken"), GetVal("accesstokensecret"),
                method, timeStamp, nonce, 
                OAuthBase.SignatureTypes.HMACSHA1, out url, out normparams);

            sig = HttpUtility.UrlEncode(sig);

            var request = HttpWebRequest.Create(uri);
            request.Method = method;
            request.Headers.Add(
                HttpRequestHeader.Authorization,
                String.Format("OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"{1}\", oauth_consumer_key=\"{2}\", oauth_token=\"{3}\", oauth_signature=\"{4}\", oauth_version=\"1.0\"",
                    nonce,
                    timeStamp,
                    oAuth.UrlEncode(GetVal("consumerkey")),
                    oAuth.UrlEncode(GetVal("accesstoken")),
                    sig
                )
            );

            return request;
        }

        protected WebRequest Post(string path, Dictionary<string, string> parameters)
        {
            return GetRequest("POST", path, parameters);
        }

        protected WebRequest Get(string path, Dictionary<string, string> parameters)
        {
            return GetRequest("GET", path, parameters);
        }

        static OAuthMedium()
        {
            System.Net.ServicePointManager.Expect100Continue = false;
        }
    }
}
