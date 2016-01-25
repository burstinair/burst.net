using System;
using System.Collections.Generic;
using System.Text;

namespace Burst.Web
{
    public class BaseException : Exception
    {
        public virtual string Status
        {
            get { return "error"; }
        }

        public virtual string ClientTip
        {
            get { return string.Format(@"{{""status"":""{0}"",""tip"":""{1}""}}", Status, Message); }
        }

        public virtual string ServerTip
        {
            get { return Message; }
        }

        public BaseException() : base("服务器发生了错误，请稍后再试。") { }
        public BaseException(Exception innerException) : base("服务器发生了错误，请稍后再试。", innerException) { }
        public BaseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
