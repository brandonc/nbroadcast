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
        public void Missing_setup_keys_throws_ArgumentException()
        {
            Twitter.Setup(new Setup()
            {
                { "consumerkey", "jgsldfghdfgu5y" },
                { "consumersecret", "593u46[po3jkryt'lsgldkjfsf,adfsdfg" },
                { "accesskey", "4528345-jahfkjsdgnpsuehtpuehgjn;SDMnspuihgs" },  // should be "accesstoken"
                { "accesskeysecret", "3573JSL;KGEPUTWPDMS;LKGJRGUITU55L2T" }     // should be "accesstokensecret"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(NoticeDispatchException))]
        public void Invalid_credentials_throws_NoticeDispatchException()
        {
            Twitter.Setup(new Setup() {
                { "consumerkey", "jgsldfghdfgu5y" },
                { "consumersecret", "593u46[po3jkryt'lsgldkjfsf,adfsdfg" },
                { "accesstoken", "4528345-jahfkjsdgnpsuehtpuehgjn;SDMnspuihgs" },  // should be "accesstoken"
                { "accesstokensecret", "3573JSL;KGEPUTWPDMS;LKGJRGUITU55L2T" }     // should be "accesstokensecret"
            });

            Notice n = new Notice("This won't be broadcast.");
            n.SetMedia(typeof(Twitter));
            n.Send();
        }

        [TestMethod]
        public void Muted_notice_does_not_throw_exception()
        {
            Twitter.Setup(new Setup() {
                { "consumerkey", "jgsldfghdfgu5y" },
                { "consumersecret", "593u46[po3jkryt'lsgldkjfsf,adfsdfg" },
                { "accesstoken", "4528345-jahfkjsdgnpsuehtpuehgjn;SDMnspuihgs" },  // should be "accesstoken"
                { "accesstokensecret", "3573JSL;KGEPUTWPDMS;LKGJRGUITU55L2T" }     // should be "accesstokensecret"
            });

            Notice n = new Notice("This won't be broadcast.");
            n.SetMedia(typeof(Twitter));
            n.Mute().Send();
        }
    }
}
