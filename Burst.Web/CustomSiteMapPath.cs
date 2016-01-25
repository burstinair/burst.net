using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Burst.Web
{
    public class CustomSiteMapPath : SiteMapPath
    {
        public Func<string, string> UrlTransform;

        protected override void InitializeItem(SiteMapNodeItem item)
        {
            if (item.SiteMapNode == null)
            {
                base.InitializeItem(item);
                return;
            }

            HyperLink hLink = new HyperLink();

            hLink.EnableTheming = false;
            hLink.Enabled = this.Enabled;

            if (UrlTransform != null)
                hLink.NavigateUrl = UrlTransform(item.SiteMapNode.Url);
            else
                hLink.NavigateUrl = item.SiteMapNode.Url;
            hLink.Text = item.SiteMapNode.Title;
            if (ShowToolTips)
                hLink.ToolTip = item.SiteMapNode.Description;

            item.Controls.Add(hLink);
        }
    }
}
