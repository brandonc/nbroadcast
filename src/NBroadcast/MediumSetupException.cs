using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBroadcast
{
    public class MediumSetupException : ApplicationException
    {
        public MediumSetupException()
            : base("Medium must be set up before it is used. Use either XML configuration or Type.Setup()")
        { }

        public MediumSetupException(string message)
            : base(message)
        { }
    }
}
