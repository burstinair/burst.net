using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst
{
    public interface IFieldInfo
    {
        string Name { get; }
        string DisplayName { get; }
        Type Type { get; }
    }
}
