using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst
{
    public interface ICustomSerializeObject
    {
        object Serialize();
        void Deserialize(object initdata);
    }
}
