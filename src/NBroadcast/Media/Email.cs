using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Threading;
using System.Net;

namespace NBroadcast.Media
{
    public class Email : Medium<Email>, IMedium
    {
        public static void Setup(Setup setup)
        {
            Medium<Email>.setup = setup;
            setup.ValidateExists("to", "from", "subject");
            setup.ValidateRange("port", 1, 65535);
        }

        internal static object AutoConfigValueHelper(string key, string value)
        {
            try
            {
                switch (key)
                {
                    case "port":
                        return Int32.Parse(value);
                    case "ssl":
                        return Boolean.Parse(value);
                    default:
                        return value;
                }
            } catch (FormatException)
            {
                return null;
            }
        }

        public void Dispatch(string body)
        {
            var message = new MailMessage(
                GetVal("from"),
                GetVal("to"),
                GetVal("subject"),
                body
            );

            var client = new SmtpClient();

            if (HasVal("server"))
                client.Host = GetVal("server");

            if (HasVal("port"))
                client.Port = GetValInt("port");

            if ((HasVal("ssl") && GetValBool("ssl")) || (!HasVal("ssl") && client.Port == 465))
            {
                client.EnableSsl = true;
            }

            if (HasVal("username"))
                client.Credentials = new NetworkCredential(
                    GetVal("username"),
                    GetValOrBlank("password")
                );

            try
            {
                client.Send(message);
            } catch (SmtpException ex)
            {
                throw new NoticeDispatchException(String.Format("The SMTP server returned an error. {0}: {1}" + ex.StatusCode.ToString(), ex.Message));
            }
        }
    }
}
