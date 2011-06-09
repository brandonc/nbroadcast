using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBroadcast.Media
{
    public interface IMedium
    {
        void Dispatch(string body);
    }
}
