using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text;
using Burst.Data.CommandBuilder;

namespace Burst.Web.UI.Controls
{
    [ParseChildren(true, "Title"), PersistChildren(false)]
    public abstract class SearchField : WebControl, INamingContainer
    {
        [PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty)]
        public string Title { get; set; }

        public string Field { get; set; }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.Write(string.Format("{0}：", Title));
            base.RenderContents(writer);
        }
        
        public abstract Compare Compare { get; }

        public abstract CompareType Type { get; }
    }
}
