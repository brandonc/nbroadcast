using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NBroadcast.Media
{
    public static class IMediumExtensions
    {
        public static void DispatchAsync(this IMedium medium, string body, BroadcastAsyncResult bar)
        {
            if (!ThreadPool.QueueUserWorkItem(
                delegate {
                    medium.Dispatch(body);
                    bar.SignalMediaComplete();
                }))
            {
                throw new InvalidOperationException("Cannot queue this notice because the thread pool was not available.");
            }
        }
    }
}
