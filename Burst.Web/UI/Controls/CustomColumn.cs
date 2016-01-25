using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing.Design;

namespace Burst.Web.UI.Controls
{
    public class CustomColumn : ColumnBase, INamingContainer
    {
        public event CreateChildrenEventHandler CreateChildren;

        public override TableCell RenderColumn(IFieldReadable Item)
        {
            var e = new CreateChildrenEventArgs();
            e.Item = Item;
            CreateChildren(this, e);
            return e.Control as TableCell;
        }
    }
}
