using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBroadcast.Media;
using System.Timers;

namespace NBroadcast
{
    public class Notice
    {
        private IMedium[] media;
        private TimeSpan delay;
        private Timer timer;

        public string Message { get; set; }

        public void Send()
        {
            if (delay != TimeSpan.Zero && delay.TotalMilliseconds > 0)
            {
                timer = new Timer(delay.TotalMilliseconds);
                timer.Elapsed += (object sender, ElapsedEventArgs e) =>
                {
                    SendImpl();
                };
                timer.Start();
            }
            else
            {
                SendImpl();
            }
        }

        private void SendImpl()
        {
            foreach (IMedium m in this.media)
            {
                m.Dispatch(this.Message);
            }
        }

        public Notice Delay(TimeSpan delay)
        {
            return new Notice(this, delay);
        }

        private Notice(Notice notice, TimeSpan delay)
        {
            this.media = notice.media;
            this.delay = delay;
        }

        public Notice(string message, params Type[] media)
        {
            if (String.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException("message");

            if (media == null || media.Length == 0)
                throw new ArgumentNullException("media");

            this.Message = message;
            this.media = new IMedium[media.Length];

            for (int index = 0; index < media.Length; index++)
            {
                this.media[index] = (IMedium)Activator.CreateInstance(media[index]);
            }
        }
    }
}
