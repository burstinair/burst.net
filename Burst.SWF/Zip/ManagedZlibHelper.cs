using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//using ManagedZlib;

namespace Burst.SWF.Zip
{
    public class ManagedZlibHelper : ZipHelperBase
    {
        public override Stream GetDefluateStream(byte[] Data)
        {
            /*
            ManagedZLib.ManagedZLib.Initialize();
            MemoryStream ms = new MemoryStream(Data);
            CompressionStream cs = new CompressionStream(ms, CompressionOptions.Decompress);
            BinaryReader reader = new BinaryReader(cs);
            Data = reader.ReadBytes();
            ManagedZLib.ManagedZLib.Terminate();
            */
            throw new NotImplementedException();
        }
    }
}
