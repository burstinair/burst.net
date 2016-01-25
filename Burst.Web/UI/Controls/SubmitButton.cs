using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Burst.Web.UI.Controls
{
    [ToolboxData("<{0}:SubmitButton runat=\"server\" />")]
    [ParseChildren(true, "Text"), PersistChildren(false)]
    public class SubmitButton : Button
    {
    }
}
