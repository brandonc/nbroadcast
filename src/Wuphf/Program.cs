using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Options;
using NBroadcast;
using NBroadcast.Media;

namespace Wuphf
{
    class Program
    {
        static void Main(string[] args)
        {
            bool show_help = false;
            string authorize = null;
            string[] services = null;
            string message = null;

            var p = new OptionSet() {
                { "h|?|help", "show this screen", v => { show_help = v != null; } },
		        { "a|authorize=", "Authorize an OAuth service", v => { authorize = v; } },
		        { "m|media=", "list of services to broadcast", v => { if(v != null) { services = v.Split(','); } } }
	        };

            List<string> extras;
            try
            {
                extras = p.Parse(args);
                if (authorize == null)
                {
                    if (extras.Count != 1 || services == null || services.Length == 0)
                        show_help = true;
                    else 
                        message = extras[0];
                } else
                {
                    if (extras.Count != 0)
                        show_help = true;
                }
            } catch (OptionException e)
            {
                Console.Write("wuphf: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `wuphf -?' for more information.");
                return;
            }

            if (show_help)
            {
                PrintUsage();
                p.WriteOptionDescriptions(Console.Out);
                return;
            }

            Setup.AutoConfig();

            if (authorize != null)
            {
                dynamic oauthmedium = null;
                if (authorize.ToLower() == "twitter")
                {
                    oauthmedium = new Twitter();
                } else if (authorize.ToLower() == "yammer")
                {
                    oauthmedium = new Yammer();
                } else
                {
                    Console.WriteLine("wuphf: Currently, I only know how to authorize Twitter and Yammer applications.");
                    return;
                }

                string consumerkey = null, consumersecret = null;
                try
                {
                    consumerkey = oauthmedium.Tokens.ConsumerKey;
                    consumersecret = oauthmedium.Tokens.ConsumerSecret;
                } catch (MediumSetupException)
                {
                    // NOP
                }

                if (String.IsNullOrEmpty(consumerkey))
                {
                    Console.Write("Consumer Key: ");
                    consumerkey = Console.ReadLine();
                }

                if (String.IsNullOrEmpty(consumersecret))
                {
                    Console.Write("Consumer Secret: ");
                    consumersecret = Console.ReadLine();
                }

                var auth = oauthmedium.Authorize(consumerkey, consumersecret);

                Console.WriteLine("Go to this url: ");
                Console.WriteLine(auth.AuthorizeUrl);

                Console.Write("And type in the code given: ");

                var access = oauthmedium.GetAccessToken(consumerkey, consumersecret, auth, Console.ReadLine());

                Console.WriteLine("Authorization complete! Use this code to configure your application:");
                Console.WriteLine();
                Console.WriteLine("{0}.Setup(new Setup() {{", oauthmedium.GetType().Name.ToString());
                Console.WriteLine("  {{ \"consumerkey\", \"{0}\" }},", access.ConsumerKey);
                Console.WriteLine("  {{ \"consumersecret\", \"{0}\" }},", access.ConsumerSecret);
                Console.WriteLine("  {{ \"accesstoken\", \"{0}\" }},", access.AccessToken);
                Console.WriteLine("  {{ \"accesstokensecret\", \"{0}\" }}", access.AccessTokenSecret);
                Console.WriteLine("});");
                Console.WriteLine();
                Console.WriteLine("...or this xml config");
                Console.WriteLine();
                Console.WriteLine("<medium name=\"{0}\">", oauthmedium.GetType().Name.ToString());
                Console.WriteLine("  <add key=\"consumerkey\" value=\"{0}\"/>", access.ConsumerKey);
                Console.WriteLine("  <add key=\"consumersecret\" value=\"{0}\"/>", access.ConsumerSecret);
                Console.WriteLine("  <add key=\"accesstoken\" value=\"{0}\"/>", access.AccessToken);
                Console.WriteLine("  <add key=\"accesstokensecret\" value=\"{0}\"/>", access.AccessTokenSecret);
                Console.WriteLine("</medium>");
                Console.WriteLine();
            } else
            {
                Notice notice = new Notice(message);
                Type[] servicestypes = new Type[services.Length];

                for(int index = 0; index < services.Length; index++) {
                    try
                    {
                        servicestypes[index] = Type.GetType("NBroadcast.Media." + services[index] + ", NBroadcast", true, true);
                    } catch (TypeLoadException)
                    {
                        Console.WriteLine("wuphf: I don't know about a medium called '" + services[index] + "'");
                        return;
                    }
                }

                notice.SetMedia(servicestypes);

                notice.Send();
            }

#if DEBUG
            Console.Write("[DEBUG] Press any key to continue");
            Console.Read();
#endif
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: wuphf (options) [message]");
        }
    }
}
