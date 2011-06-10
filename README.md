# nbroadcast #

> The .NET library for easily sending messages to a variety of internet services. Based on [broadcast by futuresimple][1] (ruby).

## Setup

Setting up channels is done statically as configuration.

### Email/SMTP ###

    Email.Setup(new Setup() {
        { "to", "brandon.croft@gmail.com" },
        { "from", "brandon.croft+nbroadcast@gmail.com" },
        { "ssl", true },
        { "subject", "Hello from your app" },
        { "server", "smtp.gmail.com" },
        { "port", 587 },
        { "username", "brandon.croft@gmail.com" },
        { "password", "secret" }
    });

### Twitter ###

    Twitter.Setup(new Setup()
    {
        { "consumerkey", "foo" },
        { "consumersecret", "bar" },
        { "accesstoken", "fiz" },
        { "accesstokensecret", "buz" }
    });

## Creating Messages

Derive simple classes from Notice that describe a message and the broadcast media to use.

    class ChainLetter : Notice
    {
        public override string Message
        {
            get { return "Send this message to 8 friends and something magical will happen!"; }
        }

        protected override Type[] GetMedia()
        {
            return new[] { typeof(Email), typeof(Twitter) /* etc... */ };
        }
    }

## Broadcast!

    new ChainLetter().Send();

## Other media:

### Jabber (XMPP) ###

    Jabber.Setup(new Setup()
    {
        { "username", "nbroadcastxmpp" },
        { "password", "secret" },
        { "server", "gmail.com" },
        { "recipient", "brandon.croft" },
        { "recipientserver", "gmail.com" }
    });

### IRC ###

    Irc.Setup(new Setup()
    {
        { "nick", "nbroadcast" },
        { "server", "irc.freenode.net" },
        { "channel", "#foo" }
    });

### Yammer (Twitter for corporations, apparently) ###

    Yammer.Setup(new Setup()
    {
        { "consumerkey", "foo" },
        { "consumersecret", "bar" },
        { "accesstoken", "fiz" },
        { "accesstokensecret", "buz" }
    });

XML config is also supported:

    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <configSections>
        <section name="nbroadcast" type="NBroadcast.Configuration.NBroadcastMediaConfigurationSection,NBroadcast"/>
      </configSections>

      <nbroadcast>
        <medium name="Twitter">
          <add key="consumerkey" value="foo"/>
          <add key="consumersecret" value="bar"/>
          <add key="accesstoken" value="fiz"/>
          <add key="accesstokensecret" value="buz"/>
        </medium>

        <medium name="Email">
          <add key="to" value="brandon.croft@gmail.com" />
          <add key="from" value="brandon.croft+nbroadcast@gmail.com" />
          <add key="ssl" value="true"/>
          <add key="subject" value="Hello from unit tests" />
          <add key="server" value="smtp.gmail.com" />
          <add key="port" value="587" />
          <add key="username" value="brandon.croft@gmail.com" />
          <add key="password" value="secret" />
        </medium>
      </nbroadcast>
    </configuration>

  [1]: https://github.com/futuresimple/broadcast