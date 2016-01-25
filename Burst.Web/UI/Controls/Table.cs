using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Burst.Web.UI.Controls
{
    [ToolboxData("<{0}:Table runat=\"server\"></{0}:Table>")]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    [ParseChildren(true, "Columns"), PersistChildren(false)]
    public class Table : CompositeControl, INamingContainer, IRepeater
    {
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(Repeater))]
        public ITemplate EmptyDataTemplate { get; set; }

        [Browsable(false)]
        public IEnumerable<IFieldReadable> Items
        {
            get { return ViewState["Items"] as IEnumerable<IFieldReadable>; }
            set
            {
                ViewState["Items"] = value;
                RecreateChildControls();
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Table;
            }
        }

        protected override void CreateChildControls()
        {
            if (Items == null || Items.Count() == 0)
            {
                if (EmptyDataTemplate != null)
                    EmptyDataTemplate.InstantiateIn(this);
            }
            else
            {
                Controls.Clear();
                foreach (var i in Items)
                {
                    var r = new TableRow();
                    foreach(var c in Columns)
                        r.Controls.Add(c.RenderColumn(i));
                    Controls.Add(r);
                }
            }
            ChildControlsCreated = true;
        }

        [Browsable(false)]
        public List<ColumnBase> Columns { get; set; }

        public string HeaderCssClass { get; set; }

        protected virtual void RenderHeader(HtmlTextWriter writer)
        {
            writer.Write("<tr>");
            foreach (var c in Columns)
            {
                string w = string.Empty;
                if (!string.IsNullOrEmpty(c.Width))
                    w = string.Format(" style=\"width:{0};\"", c.Width);
                writer.Write(string.Format("<td{0} class=\"{1}\">{2}</td>", w, HeaderCssClass, c.Title));
            }
            writer.Write("</tr>");
        }
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.Write("<tbody>");
            RenderHeader(writer);
            base.RenderContents(writer);
            writer.Write("</tbody>");
        }

        #region IRepeater 成员

        public void SetItems(IEnumerable Items)
        {
            this.Items = Items as IEnumerable<IFieldReadable>;
        }

        #endregion
    }
}