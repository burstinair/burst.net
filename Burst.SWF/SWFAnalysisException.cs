using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst.SWF
{
    public class SWFAnalysisException : Exception
    {
        public SWFAnalysisException(string Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }
}
