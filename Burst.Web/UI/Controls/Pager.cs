using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Burst.Data;
using Burst.Data.CommandBuilder;

namespace Burst.Web.UI.Controls
{
    public delegate SearchResult SearchMethod(Burst.Data.CommandBuilder.Page page, Where where, Select select, Transaction trans);

    [ToolboxData("<{0}:Pager runat=\"server\" />")]
    public class Pager : CompositeControl, INamingContainer
    {
        [Bindable(true), Category("Status"), DefaultValue(15)]
        public int PageSize
        {
            get
            {
                try
                {
                    return (int)ViewState["PageSize"];
                }
                catch
                {
                    return 15;
                }
            }
            set
            {
                ViewState["PageSize"] = value;
            }
        }

        [Bindable(true), Category("Status"), DefaultValue(1)]
        public int CurrentPage
        {
            get
            {
                try
                {
                    return (int)ViewState["CurrentPage"];
                }
                catch
                {
                    return 1;
                }
            }
            set
            {
                ViewState["CurrentPage"] = value;
            }
        }

        [Bindable(true), Category("Status"), DefaultValue(0)]
        public int TotalRecordCount
        {
            get
            {
                try
                {
                    return (int)ViewState["TotalRecordCount"];
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                ViewState["TotalRecordCount"] = value;
            }
        }

        private Control _repeater;
        [Browsable(false)]
        public Control Repeater
        {
            get
            {
                if (_repeater == null)
                    _repeater = Page.FindControl(RepeaterID);
                return _repeater;
            }
            set { _repeater = value; }
        }

        [Browsable(false)]
        public string RepeaterID { get; set; }

        [Browsable(false)]
        public Where Where { get; set; }

        [Browsable(false)]
        public Select Select { get; set; }

        [Browsable(false)]
        public Transaction Transaction { get; set; }

        [Browsable(false)]
        public SearchMethod SearchMethod { get; set; }

        internal class _pager_button : LinkButton
        {
            internal int _to_page;
        }
        
        protected bool _page_inited = false;
        protected void _page_init(int startpos)
        {
            int page_size = PageSize;
            SearchResult res = SearchMethod(new Burst.Data.CommandBuilder.Page(startpos, page_size), Where, Select, Transaction);
            TotalRecordCount = res.Total;
            CurrentPage = startpos / page_size + 1;

            if (Repeater == null)
                return;
            if (Repeater is IRepeater)
                (Repeater as IRepeater).SetItems(res.Data);
            else if (Repeater is System.Web.UI.WebControls.Repeater)
                (Repeater as System.Web.UI.WebControls.Repeater).DataSource = res.Data;
            else
                return;

            _page_inited = true;
            RecreateChildControls();
        }

        protected void _change_page(object sender, EventArgs args)
        {
            if (SearchMethod == null)
                return;

            _page_init(((sender as _pager_button)._to_page - 1) * PageSize);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!_page_inited && SearchMethod != null)
                _page_init((CurrentPage - 1) * PageSize);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Page != null)
                Page.VerifyRenderingInServerForm(this);
            base.Render(writer);
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        protected override void CreateChildControls()
        {            
            int page_size = PageSize;
            int current_page = CurrentPage;
            int total = TotalRecordCount;
            int total_page = (int)Math.Ceiling((double)total / page_size);

            _pager_button first = new _pager_button();
            first.Text = "第一页";
            first.CssClass = "first";
            first.Click += new EventHandler(_change_page);
            first._to_page = 1;
            Controls.Add(first);

            int prev_count = 2;
            for (int i = 0; i < prev_count; i++)
            {
                if (current_page - prev_count + i >= 1)
                {
                    _pager_button prev = new _pager_button();
                    prev.Text = (current_page - prev_count + i).ToString();
                    prev._to_page = current_page - prev_count + i;
                    prev.Click += new EventHandler(_change_page);
                    Controls.Add(prev);
                }
            }

            _pager_button current = new _pager_button();
            current.Text = current_page.ToString();
            current.CssClass = "selected";
            current.CommandArgument = current_page.ToString();
            current.CommandName = "ToPage";
            current._to_page = current_page;
            current.Click += new EventHandler(_change_page);
            Controls.Add(current);

            int next_count = 2;
            for (int i = 0; i < next_count; i++)
            {
                if (current_page + i + 1 <= total_page)
                {
                    _pager_button next = new _pager_button();
                    next.Text = (current_page + i + 1).ToString();
                    next._to_page = current_page + i + 1;
                    next.Click += new EventHandler(_change_page);
                    Controls.Add(next);
                }
            }

            _pager_button last = new _pager_button();
            last.Text = "最后一页";
            last.CssClass = "last";
            last._to_page = total_page;
            last.Click += new EventHandler(_change_page);
            Controls.Add(last);

            HtmlGenericControl info_panel = new HtmlGenericControl("span");
            info_panel.InnerHtml = string.Format("共&nbsp{0}&nbsp;页", total_page);
            Controls.Add(info_panel);

            HtmlGenericControl clear = new HtmlGenericControl("div");
            clear.Attributes.Add("class", "clear");
            Controls.Add(clear);

            ChildControlsCreated = true;
        }
    }
}
