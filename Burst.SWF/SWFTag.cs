using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Burst.SWF
{
    public class SWFTag
    {
        public SWFTagType Type { get; set; }
        public int Id { get; set; }
        public byte[] Content { get; set; }
        protected internal string c;

        public SWFTag(SWFTagType Type, byte[] Content)
        {
            this.Type = Type;
            this.Id = (int)Type;
            this.Content = Content;
            c = null;
        }
        public SWFTag(int Id, byte[] Content)
        {
            var tag_type = typeof(SWFTagType);
            this.Id = Id;
            this.Content = Content;
            c = null;
            if (Enum.IsDefined(tag_type, Id))
                this.Type = (SWFTagType)Id;
            else
                this.Type = SWFTagType.Unknown;
        }

        public override string ToString()
        {
            return Helper.GetString(this);
        }
    }
}
