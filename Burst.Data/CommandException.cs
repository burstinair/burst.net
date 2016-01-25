using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst.Data
{
    public class CommandException : Exception
    {
        public Command Command;
        public string MethodName;
        public string QueryString;

        public override string Message
        {
            get
            {
                return string.Format("执行Command出错。MethodName: {0}\nQueryString: {1}\n{2}", MethodName, QueryString, InnerException);
            }
        }

        public override string StackTrace
        {
            get
            {
                return  InnerException.StackTrace;
            }
        }

        public CommandException(string MethodName, Command Command, Exception InnerException)
            : base(string.Empty, InnerException)
        {
            this.MethodName = MethodName;
            this.Data.Add("MethodName", MethodName);
            this.QueryString = Command.QueryString;
            this.Data.Add("QueryString", this.QueryString);
            this.Command = Command;
            this.Data.Add("Parameters", Command.Parameters);
        }
    }
}
