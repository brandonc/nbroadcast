using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBroadcast.Media;

namespace NBroadcast.Tests
{
    [TestClass]
    public class EmailSetupTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Port_number_out_of_range_throws_ArgumentOutOfRangeException()
        {
            Email.Setup(new Setup() {
                { "to", "someone@example.com" },
                { "from", "unittest@nbroadcast.brandoncroft.com" },
                { "subject", "hello" },
                { "server", "smtp.gmail.com" },
                { "port", 65536 }
            });

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Missing_subject_throws_ArgumentException()
        {
            Email.Setup(new Setup() {
                { "to", "someone@example.com" },
                { "from", "unittest@nbroadcast.brandoncroft.com" }
            });
        }

        [TestMethod]
        [ExpectedException(typeof(NoticeDispatchException))]
        public void Invalid_credentials_throws_NoticeDispatchException()
        {
            Email.Setup(new Setup() {
                { "to", "brandon.croft@gmail.com" },
                { "from", "brandon.croft+nbroadcast@gmail.com" },
                { "ssl", true },
                { "subject", "This won't be sent" },
                { "server", "smtp.gmail.com" },
                { "port", 587 },
                { "username", "brandon.croft@gmail.com" },
                { "password", "not_my_password" }
            });

            Notice test = new Notice("This won't be sent");
            test.SetMedia(typeof(Email));
            test.Send();

        }
    }
}
