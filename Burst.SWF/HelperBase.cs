using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burst.SWF
{
    public abstract class HelperBase
    {
        public abstract string GetString(SWFFileType Type);
        public abstract string GetString(SWFTagType Type);
        public abstract string GetString(SWFFile File);
        public abstract string GetInfo(SWFTag Tag);
        public abstract string GetString(SWFTag Tag);

        public string GetHexString(byte[] bytes, int index, int endindex)
        {
            StringBuilder c = new StringBuilder();
            if (endindex > bytes.Length)
                endindex = bytes.Length;
            for (int i = index; i < endindex; i++)
                c.AppendFormat("{0} ", bytes[i].ToString("x2"));
            return c.ToString();
        }
    }
}
