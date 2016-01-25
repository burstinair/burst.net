using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Burst.Web
{
    public class XSSException : Exception
    {
        public override string Message
        {
            get
            {
                return string.Format("IP: {0}, {1}", IP, base.Message);
            }
        }

        public IPAddress IP { get; private set; }

        public XSSException(IPAddress ip, Exception InnerException)
            : base(InnerException.Message, InnerException)
        {
            this.IP = ip;
        }
    }
}
