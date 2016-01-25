using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Burst.Data.Schema
{
    public class DbSchemaException : Exception
    {
        public DbSchemaException(string Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }
}
