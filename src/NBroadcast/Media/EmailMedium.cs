using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Threading;
using System.Net;

namespace NBroadcast.Media
{
    public class EmailMedium : Medium<EmailMedium>, IMedium
    {
        public static void Setup(Setup setup)
        {
            setup.ValidateExists("to", "from", "subject");
            setup.ValidateRange("port", 1, 65535);

            Medium<EmailMedium>.setup = setup;
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

            if (HasSetup("server"))
                client.Host = GetVal("server");

            if (HasSetup("port"))
                client.Port = GetValInt("port");

            if ((HasSetup("ssl") && GetValBool("ssl")) || (!HasSetup("ssl") && client.Port == 465))
            {
                client.EnableSsl = true;
            }

            if (HasSetup("username"))
                client.Credentials = new NetworkCredential(
                    GetVal("username"),
                    GetValOrBlank("password")
                );

            client.Send(message);
        }
    }
}
