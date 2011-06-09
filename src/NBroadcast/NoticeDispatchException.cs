using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBroadcast
{
    public class NoticeDispatchException : ApplicationException
    {
        public NoticeDispatchException(string message)
            : base(message) { }
    }
}
