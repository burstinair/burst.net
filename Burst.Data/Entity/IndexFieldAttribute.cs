using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst.Data.Entity
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class IndexFieldAttribute : Attribute
    {

    }
}
