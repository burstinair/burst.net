using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Burst.Json
{
    public class JsonParseException : Exception
    {
        public JsonParseException(string Message, params object[] pms) : base(string.Format(Message, pms)) { }
    }
}