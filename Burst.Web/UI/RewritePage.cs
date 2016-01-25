using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Burst.Web
{
    public class RewritePage : System.Web.UI.Page
    {
        private string[] _rewritePathParameters;
        public string[] RewritePathParameters
        {
            get { return _rewritePathParameters; }
        }

        public RewritePage()
        {
            this.Init += new EventHandler(RewritePage_Init);
        }

        void RewritePage_Init(object sender, EventArgs e)
        {
            List<string> rewritePathParameters = new List<string>();
            string[] pms = Request.RawUrl.Split(new Char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = WebUtils.Path("/").Split(new Char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length; i < pms.Length; i++)
                rewritePathParameters.Add(pms[i]);
            this._rewritePathParameters = rewritePathParameters.ToArray();
        }
    }
}
