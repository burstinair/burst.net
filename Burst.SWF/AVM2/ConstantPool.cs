using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Burst.SWF.AVM2
{
    public class ConstantPool : Structure
    {
        public override void ReadFrom(ABCReader reader)
        {
            uint IntCount = (uint)reader.ReadEncodedInt();
            this.Integer = new Int32[IntCount];
            this.Integer[0] = 0;
            for (int i = 1; i < IntCount; i++)
                this.Integer[i] = reader.ReadEncodedInt();

            uint UintCount = (uint)reader.ReadEncodedInt();
            this.Uinteger = new UInt32[UintCount];
            this.Uinteger[0] = 0;
            for (int i = 1; i < UintCount; i++)
                this.Uinteger[i] = (uint)reader.ReadEncodedInt();

            uint DoubleCount = (uint)reader.ReadEncodedInt();
            this.Double = new Double?[DoubleCount];
            this.Double[0] = null;
            for (int i = 1; i < DoubleCount; i++)
                this.Double[i] = reader.ReadDouble();

            uint StringCount = (uint)reader.ReadEncodedInt();
            this.String = new String[StringCount];
            this.String[0] = "*";
            for (int i = 1; i < StringCount; i++)
                this.String[i] = reader.ReadString();

            uint NamespaceCount = (uint)reader.ReadEncodedInt();
            this.Namespace = new Namespace[NamespaceCount];
            this.Namespace[0] = Burst.SWF.AVM2.Namespace.PublicNs;
            for (int i = 1; i < NamespaceCount; i++)
            {
                this.Namespace[i] = new Namespace();
                this.Namespace[i].ReadFrom(reader);
            }

            uint NsSetCount = (uint)reader.ReadEncodedInt();
            this.NsSet = new NsSet[NsSetCount];
            this.NsSet[0] = null;
            for (int i = 1; i < NsSetCount; i++)
            {
                this.NsSet[i] = new NsSet();
                this.NsSet[i].ReadFrom(reader);
            }

            uint MultinameCount = (uint)reader.ReadEncodedInt();
            this.Multiname = new Multiname[MultinameCount];
            this.Multiname[0] = null;
            for (int i = 1; i < MultinameCount; i++)
            {
                this.Multiname[i] = new Multiname();
                this.Multiname[i].ReadFrom(reader);
            }
        }

        public void Initialize()
        {
            foreach (var ns in Namespace)
                ns.Initialize(this);
            foreach (var nsset in NsSet)
                nsset.Initialize(this);
            foreach (var mn in Multiname)
                mn.Initialize(this);
        }

        public Int32[] Integer
        {
            get { return (Int32[])this["integer"]; }
            set { this["integer"] = value; }
        }
        public UInt32[] Uinteger
        {
            get { return (UInt32[])this["uinteger"]; }
            set { this["uinteger"] = value; }
        }
        public Double?[] Double
        {
            get { return (Double?[])this["double"]; }
            set { this["double"] = value; }
        }
        public String[] String
        {
            get { return (String[])this["string"]; }
            set { this["string"] = value; }
        }
        public Namespace[] Namespace
        {
            get { return (Namespace[])this["namespace"]; }
            set { this["namespace"] = value; }
        }
        public NsSet[] NsSet
        {
            get { return (NsSet[])this["ns_set"]; }
            set { this["ns_set"] = value; }
        }
        public Multiname[] Multiname
        {
            get { return (Multiname[])this["multiname"]; }
            set { this["multiname"] = value; }
        }
    }
}
