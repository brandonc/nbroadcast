using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NBroadcast
{
    public class BroadcastAsyncResult : IAsyncResult
    {
        private ManualResetEvent waitHandle;
        private static object asyncState = new object();
        private int mediaCount = 0;
        private int mediaComplete = 0;

        public double PercentComplete
        {
            get
            {
                return ((mediaCount == 0 ? 100.0 : (double)mediaComplete / (double)mediaCount)) * 100.0;
            }
        }

        internal void SignalMediaComplete()
        {
            this.mediaComplete++;
            if (IsCompleted)
                waitHandle.Set();
        }

        internal BroadcastAsyncResult(Type[] media)
        {
            mediaCount = media.Length;
            waitHandle = new ManualResetEvent(false);
        }

        public object AsyncState
        {
            get { return asyncState; }
        }

        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get { return waitHandle; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get { return this.PercentComplete == 100; }
        }
    }
}
