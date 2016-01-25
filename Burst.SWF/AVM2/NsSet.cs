using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Burst.SWF.AVM2
{
    public class NsSet : Structure
    {
        public int Count
        {
            get { return (int)this["count"]; }
            set { this["count"] = value; }
        }
        public int[] NsIndex
        {
            get { return (int[])this["ns"]; }
            set { this["ns"] = value; }
        }

        public Namespace[] Ns { get; set; }

        public override void ReadFrom(ABCReader reader)
        {
            this.Count = reader.ReadEncodedInt();
            this.NsIndex = new int[Count];
            for (int i = 0; i < Count; i++)
                this.NsIndex[i] = reader.ReadEncodedInt();
        }

        public void Initialize(ConstantPool Pool)
        {
            this.Ns = new Namespace[Count];
            for (int i = 0; i < Count; i++)
                this.Ns[i] = Pool.Namespace[this.NsIndex[i]];
        }
    }
}
