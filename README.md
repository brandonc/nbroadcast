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

## Building from source

    Run build.bat (MSBuild required). Release binaries are copied to the binaries/ folder.

## Authorization

nbroadcast includes an OAuth authorization helper to authorize your apps for Twitter and Yammer. Once you have a consumer key and consumer secret, use the command line utility wuphf.exe:

    Usage: wuphf (options) [message]
      -a, --authorize=(Twitter|Yammer)      Authorize an OAuth service

Follow the instructions to obtain an access token and access token secret.

## LICENSE

The MIT License

Copyright (c) 2011 Brandon Croft and contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

  [1]: https://github.com/futuresimple/broadcast
