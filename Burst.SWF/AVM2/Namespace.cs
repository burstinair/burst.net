using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Burst.SWF.AVM2
{
    public enum NamespaceKind
    {
        Namespace = 0x08,
        PackageNamespace = 0x16,
        PackageInternalNs = 0x17,
        ProtectedNamespace = 0x18,
        ExplicitNamespace = 0x19,
        StaticProtectedNs = 0x1A,
        PrivateNs = 0x05
    }

    public class Namespace : Structure
    {
        public static readonly Namespace PublicNs = new Namespace(string.Empty);
        public static readonly Namespace AnyNs = new Namespace("*");

        public NamespaceKind Kind
        {
            get { return (NamespaceKind)this["kind"]; }
            set { this["kind"] = value; }
        }
        public int NameIndex
        {
            get { return (int)this["name"]; }
            set { this["name"] = value; }
        }

        public Namespace()
        { }
        public Namespace(string Name)
        {
            this.Name = Name;
        }

        public string Name { get; set; }

        public override void ReadFrom(ABCReader reader)
        {
            this.Kind = (NamespaceKind)reader.ReadByte();
            this.NameIndex = reader.ReadEncodedInt();
        }

        public void Initialize(ConstantPool Pool)
        {
            if (this.NameIndex == 0)
                this.Name = string.Empty;
            else
                this.Name = Pool.String[this.NameIndex];
        }
    }
}
