using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Burst;
using Burst.Data;
using Burst.Data.CommandBuilder;

namespace Burst.Web.UI.Controls
{
    [ToolboxData("<{0}:Searcher runat=\"server\"></{0}:Searcher>")]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    [ParseChildren(true, "ParsedControls"), PersistChildren(false)]
    public class Searcher : CompositeControl, INamingContainer
    {
        private Pager _pager;
        [Browsable(false)]
        public Pager Pager
        {
            get
            {
                if (_pager == null)
                    _pager = Page.FindControl(PagerID) as Pager;
                return _pager;
            }
            set { _pager = value; }
        }

        [Browsable(false)]
        public string PagerID { get; set; }

        [Bindable(true), Category("Searcher"), DefaultValue("")]
        public string Tables
        {
            get
            {
                return (string)ViewState["Tables"];
            }
            set
            {
                ViewState["Tables"] = value;
            }
        }

        [Browsable(false)]
        public List<System.Web.UI.Control> ParsedControls { get; set; }

        private List<SearchField> _fields;
        [Browsable(false)]
        public List<SearchField> Fields
        {
            get
            {
                if (_fields == null)
                {
                    _fields = new List<SearchField>();
                    foreach (var i in ParsedControls)
                        if (i is SearchField)
                            _fields.Add(i as SearchField);
                }
                return _fields;
            }
        }

        protected bool _searcher_inited = false;
        protected void _searcher_init()
        {
            if (Pager == null)
                return;
            Pager.Where = (from field in Fields select field.Compare).ToWhere();

            _searcher_inited = true;
            RecreateChildControls();
        }

        protected void _search_btn_click(object sender, EventArgs args)
        {
            _searcher_init();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!_searcher_inited)
                _searcher_init();
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
            foreach(var field in ParsedControls)
                Controls.Add(field);

            Button search_btn = new Button();
            search_btn.Text = "搜索";
            search_btn.Click += new EventHandler(_search_btn_click);
            Controls.Add(search_btn);

            ChildControlsCreated = true;
        }
    }
}
