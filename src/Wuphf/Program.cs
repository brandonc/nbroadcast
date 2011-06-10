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

            if (authorize != null)
            {
                if (authorize.ToLower() == "twitter")
                {
                    throw new NotImplementedException();
                } else if (authorize.ToLower() == "yammer")
                {
                    throw new NotImplementedException();
                } else
                {
                    Console.WriteLine("wuphf: Currently, I only know how to authorize Twitter and Yammer applications.");
                    return;
                }
            } else
            {
                Setup.AutoConfig();

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
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: wuphf (options) [message]");
        }
    }
}
