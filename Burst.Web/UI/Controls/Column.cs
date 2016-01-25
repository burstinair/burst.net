using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Burst.Web.UI.Controls
{
    public class Column : ColumnBase, INamingContainer
    {
        public string Field { get; set; }

        public override TableCell RenderColumn(IFieldReadable Item)
        {
            var res = new TableCell();
            res.Text = Convert.ToString(Item.GetFieldValue(Field));
            return res;
        }
    }
}
