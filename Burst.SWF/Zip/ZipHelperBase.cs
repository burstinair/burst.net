using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Burst.SWF.Zip
{
    public abstract class ZipHelperBase
    {
        public abstract Stream GetDefluateStream(byte[] Data);
    }
}
