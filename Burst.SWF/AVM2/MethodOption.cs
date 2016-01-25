using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Burst.SWF.AVM2
{
    public class MethodOption : Structure
    {
        public enum MethodOptionKind
        {
            Int = 0x03,
            UInt = 0x04,
            Double = 0x06,
            Utf8 = 0x01,
            True = 0x0B,
            False = 0x0A,
            Null = 0x0C,
            Undefined = 0x00,
            Namespace = 0x08,
            PackageNamespace = 0x16,
            PackageInternalNs = 0x17,
            ProtectedNamespace = 0x18,
            ExplicitNamespace = 0x19,
            StaticProtectedNs = 0x1A,
            PrivateNs = 0x05
        }

        public override void ReadFrom(ABCReader reader)
        {
            throw new NotImplementedException();
        }

        public UInt32 OptionCount
        {
            get { return (UInt32)this["option_count"]; }
            set { this["option_count"] = value; }
        }
        public UInt32[] Val
        {
            get { return (UInt32[])this["val"]; }
            set { this["val"] = value; }
        }
        public Byte[] Kind
        {
            get { return (Byte[])this["kind"]; }
            set { this["kind"] = value; }
        }
    }
}
