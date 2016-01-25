using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ionic.Zlib;

namespace Burst.SWF.Zip
{
    public class IonicZlibHelper : ZipHelperBase
    {
        public override Stream GetDefluateStream(byte[] Data)
        {
            var msSinkDecompressed = new System.IO.MemoryStream(Data);
            return new ZlibStream(msSinkDecompressed, CompressionMode.Decompress, true);
        }
    }
}
