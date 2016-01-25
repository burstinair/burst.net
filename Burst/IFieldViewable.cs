using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst
{
    public interface IFieldViewable
    {
        IEnumerable<IFieldInfo> Fields { get; }
    }
}
