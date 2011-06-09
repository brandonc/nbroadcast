using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBroadcast
{
    public class MediumSetupException : ApplicationException
    {
        public MediumSetupException()
            : base("Medium is not setup.")
        { }

        public MediumSetupException(string message)
            : base(message)
        { }
    }
}
