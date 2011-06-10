using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaBuilders.Irc;
using System.Threading;

namespace NBroadcast.Media
{
    public class Irc : Medium<Irc>, IMedium
    {
        private const int TIMEOUT_SECONDS = 30;

        public static void Setup(Setup setup)
        {
            setup.ValidateExists("nick", "server", "channel");
            setup.ValidateRegex("channel", "^#.+");

            Medium<Irc>.setup = setup;
        }

        public void Dispatch(string body)
        {
            var client = new Client(GetVal("server"), GetVal("nick"));
            client.EnableAutoIdent = false;

            bool finished = false;

            client.Messages.Welcome += delegate(object welcomesender, MetaBuilders.Irc.Messages.IrcMessageEventArgs<MetaBuilders.Irc.Messages.WelcomeMessage> e)
            {
                client.SendJoin(GetVal("channel"));
            };

            client.Messages.Join += delegate(object joinsender, MetaBuilders.Irc.Messages.IrcMessageEventArgs<MetaBuilders.Irc.Messages.JoinMessage> e)
            {
                client.SendChat(body, GetVal("channel"));
                finished = true;
                client.SendQuit();
            };

            client.Messages.GenericErrorMessage += delegate(object errorsender, MetaBuilders.Irc.Messages.IrcMessageEventArgs<MetaBuilders.Irc.Messages.GenericErrorMessage> e)
            {
                finished = true;
                throw new NoticeDispatchException("Could not connect to IRC: " + e.Message.ToString());
            };

            client.Connection.Connect();
            
            DateTime start = DateTime.Now;
            while (!finished && (DateTime.Now - start).TotalSeconds < TIMEOUT_SECONDS)
            {
                Thread.Sleep(100);
            }

            if (!finished)
            {
                throw new NoticeDispatchException("IRC login timeout");
            }
        }
    }
}
