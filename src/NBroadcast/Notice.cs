
// The notice class can be used directly or subclassed. A notice defines it's broadcast channels and the message to be broadcast.
//
// When subclassed, override `Message` and `GetMedia` to define broadcast channels and the message.
//
//     class ChainLetter : Notice
//     {
//         public override string Message
//         {
//             get { return @"Send this message to 8 friends and
//                            something magical will happen!"; }
//         }
//
//         protected override Type[] GetMedia()
//         {
//             return new[] { typeof(Email), typeof(Twitter)
//                            /* etc... */ };
//         }
//     }

using System;
using System.Timers;
using NBroadcast.Media;

namespace NBroadcast
{
    public class Notice : ICloneable
    {
        private TimeSpan noticedelay;
        private string message;
        private Type[] media;

        // If `MuteExceptions` is enabled, no dispatch exceptions are raised when broadcasts fail.
        public bool MuteExceptions { get; set; }

        // The message to be sent. Override this if subclassing.
        public virtual string Message
        {
            get { return this.message; }
        }

        // Mute returns a copy of this notice except the copy does not raise exceptions when broadcasts fail.
        //
        //     var notice = new ChainLetter()l
        //     notice.Mute().Send(); // Muted delivery
        //     notice.Send();        // Non-muted delivery
        public Notice Mute()
        {
            var result = (Notice)this.Clone();
            result.MuteExceptions = true;
            return result;
        }

        // Delay returns a copy of this notice except dispatches from the copy are delayed by the specified amount of time.
        //
        //     var notice = new ChainLetter()l
        //     notice.Delay(TimeSpan.FromSeconds(60)).Send(); // Delayed
        //     notice.Send();                                 // Instant
        public Notice Delay(TimeSpan delay)
        {
            var result = (Notice)this.Clone();
            result.noticedelay = delay;
            return result;
        }

        // If *not* subclassing, use `SetMedia` to define broadcast media.
        //
        //     Notice n = new Notice("Hello, World!");
        //     n.SetMedia(typeof(Twitter), typeof(Email));
        //     n.Send();
        public void SetMedia(params Type[] media)
        {
            if (media == null)
                throw new ArgumentNullException("media");

            this.media = media;
        }

        // If subclassing, override this method to define broadcast media.
        protected virtual Type[] GetMedia()
        {
            if (media == null && !this.MuteExceptions)
                throw new NoticeDispatchException("No media has been defined.");

            return this.media == null ? new Type[0] : this.media;
        }

        // Synchronous delivery uses any delay previously specified
        public void Send()
        {
            DelayInternal(this.noticedelay, () =>
            {
                SendInternal(null);
            });
        }

        // Asynchronous deliver uses the application thread pool. If the application quits, remaining delivery is cancelled.
        public BroadcastAsyncResult SendAsync()
        {
            var ar = new BroadcastAsyncResult(this.GetMedia());

            DelayInternal(this.noticedelay, () => {
                SendInternal(ar);
            });

            return ar;
        }

        // Synchronous delivery after a specified delay
        public void SendDelayed(TimeSpan delay)
        {
            DelayInternal(delay, () =>
            {
                SendInternal(null);
            });
        }

        private void DelayInternal(TimeSpan delay, Action callback)
        {
            if (delay != TimeSpan.Zero && delay.TotalMilliseconds > 0)
            {
                Timer timer = new Timer(delay.TotalMilliseconds);
                timer.Elapsed += (object sender, ElapsedEventArgs e) =>
                {
                    callback();
                };
                timer.Start();
            } else
            {
                callback();
            }
        }

        private void SendInternal(BroadcastAsyncResult ar)
        {
            foreach (Type tm in this.GetMedia())
            {
                var m = (IMedium)Activator.CreateInstance(tm);
                try
                {
                    if (ar != null)
                        m.DispatchAsync(this.Message, ar);
                    else
                        m.Dispatch(this.Message);
                } catch (NoticeDispatchException)
                {
                    if (!MuteExceptions)
                        throw;
                }
            }
        }

        public Notice(string message)
        {
            this.message = message;
        }

        protected Notice() { }

        public object Clone()
        {
            return new Notice()
            {
                media = this.media,
                message = this.message,
                noticedelay = this.noticedelay,
                MuteExceptions = this.MuteExceptions
            };  
        }
    }
}
