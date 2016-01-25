using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Burst.SWF.AVM2
{
    public class Method : Structure
    {
        public override void ReadFrom(ABCReader reader)
        {
            uint ParamCount = (uint)reader.ReadEncodedInt();
            this.ReturnTypeIndex = (UInt32)reader.ReadEncodedInt();

            this.ParamType = new UInt32[ParamCount];
            for (int i = 0; i < ParamCount; i++)
                this.ParamType[i] = (UInt32)reader.ReadEncodedInt();

            this.NameIndex = (UInt32)reader.ReadEncodedInt();

            this.Flags = reader.ReadByte();

            this.Options.ReadFrom(reader);

            this.ParamNames.ReadFrom(reader);
        }

        public void Initialize(ConstantPool Pool)
        {
            if (ReturnTypeIndex == 0)
                ReturnType = Multiname.AnyName;
            else
                ReturnType = Pool.Multiname[ReturnTypeIndex];

            if (NameIndex == 0)
                Name = null;
            else
                Name = Pool.String[NameIndex];


        }

        public Multiname ReturnType { get; set; }
        public string Name { get; set; }

        public UInt32 ReturnTypeIndex
        {
            get { return (UInt32)this["return_type"]; }
            set { this["return_type"] = value; }
        }
        public UInt32[] ParamType
        {
            get { return (UInt32[])this["param_type"]; }
            set { this["param_type"] = value; }
        }
        public UInt32 NameIndex
        {
            get { return (UInt32)this["name"]; }
            set { this["name"] = value; }
        }
        public Byte Flags
        {
            get { return (Byte)this["flags"]; }
            set { this["flags"] = value; }
        }
        public Option Options
        {
            get { return (Option)this["options"]; }
            set { this["options"] = value; }
        }
        public Param ParamNames
        {
            get { return (Param)this["param_names"]; }
            set { this["param_names"] = value; }
        }
    }
}
