using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst.SWF
{
    public class BitArray
    {
        protected byte[] bytes;

        public int Length
        {
            get
            {
                return bytes.Length * 8;
            }
        }

        public BitArray(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public int this[int index]
        {
            get
            {
                int x = index / 8;
                int y = index % 8;
                return bytes[x] >> (7 - y) & 1;
            }
        }

        public int GetInt(int index, int count)
        {
            int res = 0, d = 1;
            for (int i = 0; i < count; i++)
            {
                int x = index + count - i - 1;
                res += this[x] * d;
                d *= 2;
            }
            return res;
        }
    }
}
