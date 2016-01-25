using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Burst.SWF.AVM2
{
    public class ABCReader : BinaryReader
    {
        public ABCReader(Stream s)
            : base(s)
        {
        }

        public int ReadEncodedInt()
        {
            return base.Read7BitEncodedInt();
        }
        public int ReadInt24()
        {
            return BitConverter.ToInt32(ReadBytes(3), 0);
        }
    }
}
