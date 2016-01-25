using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Burst.Data.Entity
{
    public class DataEntityException : Exception
    {
        public DataEntityException(string Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }
}
