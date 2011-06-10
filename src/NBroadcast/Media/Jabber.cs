using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.component;
using agsXMPP.protocol.client;
using System.Threading;

namespace NBroadcast.Media
{
    public class Jabber : Medium<Jabber>, IMedium
    {
        private const int TIMEOUT_SECONDS = 10;

        public static void Setup(Setup setup)
        {
            setup.ValidateExists("username", "server", "password", "recipient", "recipientserver");
            Medium<Jabber>.setup = setup;
        }

        public void Dispatch(string body)
        {
            XmppClientConnection client = new XmppClientConnection(GetVal("server"));
            client.AutoResolveConnectServer = true;
            client.UseSSL = false;
            client.UseStartTLS = true;
            client.RegisterAccount = false;
            client.AutoAgents = false;

            bool finished = false;
            client.OnAuthError += delegate(object autherrorsender, agsXMPP.Xml.Dom.Element e)
            {
                finished = true;
                throw new NoticeDispatchException("XMPP could not authenticate");
            };

            client.OnError += delegate(object errorsender, Exception ex) {
                finished = true;
                throw new NoticeDispatchException("XMPP exception: " + ex.Message);
            };

            client.OnSocketError += delegate(object socketerrorsender, Exception ex)
            {
                finished = true;
                throw new NoticeDispatchException("XMPP socket exception: " + ex.Message);
            };

            client.OnLogin += delegate(object loginsender)
            {
                client.Send(new agsXMPP.protocol.client.Message(new Jid(GetVal("recipient"), GetVal("recipientserver"), null), MessageType.chat, body));
                client.Close();
                finished = true;
            };

            client.Open(GetVal("username"), GetVal("password"));

            DateTime start = DateTime.Now;
            while (!finished && (DateTime.Now - start).TotalSeconds < TIMEOUT_SECONDS)
            {
                Thread.Sleep(100);
            }

            if (!finished)
            {
                throw new NoticeDispatchException("XMPP login timeout");
            }
        }
    }
}
