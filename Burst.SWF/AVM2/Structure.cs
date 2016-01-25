using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Burst.SWF.AVM2
{
    public abstract class Structure : NameValueList
    {
        public abstract void ReadFrom(ABCReader reader);
        public void ReadFrom(Stream stream)
        {
            using (var reader = new ABCReader(stream))
                ReadFrom(reader);
        }
        public void ReadFrom(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                ReadFrom(ms);
            }
        }
    }
}
