using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text;
using Burst.Web;

namespace Burst.Web.UI.Controls
{
    public class Tip : CompositeControl, INamingContainer
    {
        private BaseException _ex;

        public void Show(BaseException ex)
        {
            _ex = ex;
        }

        public void Hide()
        {
            _ex = null;
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (_ex != null)
            {
                writer.Write(string.Format("<span class='{0}'>{1}</span>", _ex.Status, _ex.ServerTip));
                this.Visible = true;
            }
            else
                this.Visible = false;
        }
    }
}
