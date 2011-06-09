using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using OAuth;
using System.Web;

namespace NBroadcast.Media
{
    public class OAuthMedium<T> : Medium<T>
    {
        protected string apiroot;

        private WebRequest GetRequest(string method, string path, Dictionary<string, string> param) {
            string url, normparams;

            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var pair in param)
            {
                if (!first)
                {
                    sb.Append("&");
                }
                first = false;
                sb.AppendFormat("{0}={1}", pair.Key, System.Web.HttpUtility.UrlEncode(pair.Value));
            }

            Uri uri = new Uri("http://" + apiroot + path + "?" + sb.ToString());

            OAuthBase oAuth = new OAuthBase();
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
            request.Headers.Add(HttpRequestHeader.Authorization, String.Format("OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", oauth_timestamp=\"{2}\", oauth_signature_method=\"HMAC-SHA1\", oauth_signature=\"{3}\", oauth_version=\"1.0\"", HttpUtility.UrlEncode(GetVal("consumerkey")), nonce, timeStamp, sig));

            if (method == "POST")
            {
                byte[] postData = Encoding.UTF8.GetBytes(sb.ToString());

                request.ContentLength = postData.Length;
                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(postData, 0, postData.Length);
                }
            }

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
