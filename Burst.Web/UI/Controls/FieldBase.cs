using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Burst.Web.UI.Controls
{
    [ParseChildren(true, "Title"), PersistChildren(false)]
    public abstract class FieldBase : CompositeControl, INamingContainer, IField
    {
        [PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty)]
        public string Title { get; set; }

        protected IFieldReadable Item;
        private HtmlTextWriterTag Tag;
        protected internal void Initialize(IFieldReadable Item, HtmlTextWriterTag Tag)
        {
            this.Item = Item;
            this.Tag = Tag;
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return this.Tag;
            }
        }

        public abstract void Update(IFieldWritable Item);
        public abstract IEnumerable<string> UpdateFields { get; }
    }
}
