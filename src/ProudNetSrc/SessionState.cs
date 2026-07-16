using System;
using System.Collections.Generic;
using System.Text;

namespace ProudNetSrc
{
    internal enum SessionState
    {
        Handshake,
        HandshakeKeyExchanged,
        Connected
    }
}
