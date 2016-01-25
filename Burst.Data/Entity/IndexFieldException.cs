using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst.Data.Entity
{
    public class IndexFieldException : Exception
    {
        public IndexFieldException(string Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }
}
