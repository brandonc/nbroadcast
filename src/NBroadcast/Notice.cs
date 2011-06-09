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
        private TimeSpan delay;
        private Timer timer;
        private string message;
        private Type[] media;

        public bool MuteExceptions { get; set; }

        public virtual string Message
        {
            get
            {
                return this.message;
            }
        }

        public void SetMedia(params Type[] media)
        {
            if (media == null)
                throw new ArgumentNullException("media");

            this.media = media;
        }

        protected virtual Type[] GetMedia()
        {
            return this.media;
        }

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
            
                foreach (Type tm in this.GetMedia())
                {
                    var m = (IMedium)Activator.CreateInstance(tm);
                    try
                    {
                        m.Dispatch(this.Message);
                    } catch (NoticeDispatchException)
                    {
                        if (!MuteExceptions)
                            throw;
                    }
                }
        }

        public Notice Mute()
        {
            this.MuteExceptions = true;
            return this;
        }

        public Notice Delay(TimeSpan delay)
        {
            this.delay = delay;
            return this;
        }

        public Notice(string message)
        {
            this.message = message;
        }

        protected Notice() { }
    }
}
