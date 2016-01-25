using System;
using System.Collections.Generic;
using System.Text;

namespace Burst
{
    public interface ISerializer
    {
        object Serialize(object oriobj);
        object Deserialize(object data, Type type);
    }
}
