using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBroadcast.Media;

namespace NBroadcast.Tests
{
    [TestClass]
    public class TwitterSetupTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MissingKeysTests()
        {
            Twitter.Setup(new Setup()
            {
                { "consumerkey", "jgsldfghdfgu5y" },
                { "consumersecret", "593u46[po3jkryt'lsgldkjfsf,adfsdfg" },
                { "accesskey", "4528345-jahfkjsdgnpsuehtpuehgjn;SDMnspuihgs" },  // should be "accesstoken"
                { "accesskeysecret", "3573JSL;KGEPUTWPDMS;LKGJRGUITU55L2T" }     // should be "accesstokensecret"
            });
        }
    }
}
