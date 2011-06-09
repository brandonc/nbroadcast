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
        public void PortNumberOutOfRangeTest()
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
        public void MissingSubjectTest()
        {
            Email.Setup(new Setup() {
                { "to", "someone@example.com" },
                { "from", "unittest@nbroadcast.brandoncroft.com" }
            });
        }
    }
}
