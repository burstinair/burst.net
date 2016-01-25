using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Burst.SWF.AVM2
{
    public enum MultiNameKind
    {
        QName = 0x07,
        QNameA = 0x0D,
        NameL = 0x13,
        NameLA = 0x14,
        RTQName = 0x0F,
        RTQNameA = 0x10,
        RTQNameL = 0x11,
        RTQNameLA = 0x12,
        Multiname = 0x09,
        MultinameA = 0x0E,
        MultinameL = 0x1B,
        MultinameLA = 0x1C,
        TypeName = 0x1D
    }

    public class Multiname : Structure
    {
        public static readonly Multiname PublicName = new Multiname(Namespace.PublicNs, string.Empty);
        public static readonly Multiname AnyName = new Multiname(Namespace.PublicNs, "*");

        public Multiname()
        { }
        public Multiname(Namespace Ns, string Name)
        {
            this.Kind = MultiNameKind.QName;
            this.Ns = Ns;
            this.Name = Name;
        }

        public MultiNameKind Kind
        {
            get { return (MultiNameKind)this["kind"]; }
            set { this["kind"] = value; }
        }
        public int NameIndex
        {
            get { return (int)this["name"]; }
            set { this["name"] = value; }
        }
        public int NsIndex
        {
            get { return (int)this["ns"]; }
            set { this["ns"] = value; }
        }
        public int NsSetIndex
        {
            get { return (int)this["ns_set"]; }
            set { this["ns_set"] = value; }
        }
        public int[] TypeIndex
        {
            get { return (int[])this["type"]; }
            set { this["type"] = value; }
        }

        public string Name { get; set; }
        public Namespace Ns { get; set; }
        public NsSet NsSet { get; set; }
        public string[] Types { get; set; }

        public string QName
        {
            get
            {
                switch (Kind)
                {
                    case MultiNameKind.QName:
                    case MultiNameKind.QNameA:
                        return string.Format("{0}.{1}", Ns.Name, Name);
                    case MultiNameKind.RTQName:
                    case MultiNameKind.RTQNameA:
                        return Name;
                    case MultiNameKind.RTQNameL:
                    case MultiNameKind.RTQNameLA:
                        return null;
                    case MultiNameKind.NameL:
                    case MultiNameKind.NameLA:
                        return string.Empty;
                    case MultiNameKind.Multiname:
                    case MultiNameKind.MultinameA:
                    case MultiNameKind.MultinameL:
                    case MultiNameKind.MultinameLA:
                        var res = new StringBuilder();
                        foreach (var ns in NsSet.Ns)
                            res.AppendFormat("{0}.", ns.Name);
                        res.Append(Name);
                        return res.ToString();
                    default:
                        throw new SWFAnalysisException("Invalid Multiname Kind.", null);
                }
            }
        }

        public override void ReadFrom(ABCReader reader)
        {
            this.Kind = (MultiNameKind)reader.ReadByte();
            switch (Kind)
            {
                case MultiNameKind.QName:
                case MultiNameKind.QNameA:
                    NsIndex = reader.ReadEncodedInt();
                    NameIndex = reader.ReadEncodedInt();
                    break;
                case MultiNameKind.RTQName:
                case MultiNameKind.RTQNameA:
                    NameIndex = reader.ReadEncodedInt();
                    break;
                case MultiNameKind.RTQNameL:
                case MultiNameKind.RTQNameLA:
                case MultiNameKind.NameL:
                case MultiNameKind.NameLA:
                    break;
                case MultiNameKind.Multiname:
                case MultiNameKind.MultinameA:
                    NameIndex = reader.ReadEncodedInt();
                    NsSetIndex = reader.ReadEncodedInt();
                    break;
                case MultiNameKind.MultinameL:
                case MultiNameKind.MultinameLA:
                    NsSetIndex = reader.ReadEncodedInt();
                    break;
                case MultiNameKind.TypeName:
                    NameIndex = reader.ReadEncodedInt();
                    var count = reader.ReadEncodedInt();
                    this.TypeIndex = new int[count];
                    for (int i = 0; i < count; i++)
                        this.TypeIndex[i] = reader.ReadEncodedInt();
                    break;
                default:
                    throw new SWFAnalysisException("Invalid Multiname Kind.", null);
            }
        }

        private void InitializeName(ConstantPool Pool)
        {
            if (NameIndex == 0)
                Name = "*";
            else
                Name = Pool.String[NameIndex];
        }
        private void InitializeNamespace(ConstantPool Pool)
        {
            if (NsIndex == 0)
                Ns = Namespace.AnyNs;
            else
                Ns = Pool.Namespace[NsIndex];
        }
        private void InitializeNsSet(ConstantPool Pool)
        {
            if (NsSetIndex == 0)
                throw new SWFAnalysisException("NsSetIndex cannot be zero.", null);
            else
                NsSet = Pool.NsSet[NsIndex];
        }
        public void Initialize(ConstantPool Pool)
        {
            switch (Kind)
            {
                case MultiNameKind.QName:
                case MultiNameKind.QNameA:
                    InitializeName(Pool);
                    InitializeNamespace(Pool);
                    break;
                case MultiNameKind.RTQName:
                case MultiNameKind.RTQNameA:
                    InitializeName(Pool);
                    break;
                case MultiNameKind.RTQNameL:
                case MultiNameKind.RTQNameLA:
                case MultiNameKind.NameL:
                case MultiNameKind.NameLA:
                    break;
                case MultiNameKind.Multiname:
                case MultiNameKind.MultinameA:
                    InitializeName(Pool);
                    InitializeNsSet(Pool);
                    break;
                case MultiNameKind.MultinameL:
                case MultiNameKind.MultinameLA:
                    InitializeNsSet(Pool);
                    break;
                case MultiNameKind.TypeName:
                    InitializeName(Pool);
                    for (int i = 0; i < this.TypeIndex.Length; i++)
                    {
                        if (this.TypeIndex[i] == 0)
                            this.Types[i] = string.Empty;
                        else
                            this.Types[i] = Pool.String[this.TypeIndex[i]];
                    }
                    break;
                default:
                    throw new SWFAnalysisException("Invalid Multiname Kind.", null);
            }
        }
    }
}
