using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Burst.Web.UI.Controls
{
    [ParseChildren(true, "Title"), PersistChildren(false)]
    public abstract class ColumnBase : WebControl, INamingContainer
    {
        [PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty)]
        public string Title { get; set; }

        public abstract TableCell RenderColumn(IFieldReadable Item);
        
        public new string Width { get; set; }
    }
}
