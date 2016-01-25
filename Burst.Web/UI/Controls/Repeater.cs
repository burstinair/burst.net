using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Burst.Web.UI.Controls
{
    public delegate HtmlGenericControl GenerateMethod(int index, object data);

    [ToolboxData("<{0}:Repeater runat=\"server\" />")]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    [ParseChildren(true), PersistChildren(false)]
    public class Repeater : CompositeControl, INamingContainer, IRepeater
    {
        [Browsable(false)]
        public GenerateMethod GenerateMethod { get; set; }

        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(Repeater))]
        public ITemplate EmptyDataTemplate { get; set; }

        [Browsable(false)]
        public IEnumerable Items
        {
            get { return ViewState["Items"] as IEnumerable; }
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
                return HtmlTextWriterTag.Div;
            }
        }

        protected override void CreateChildControls()
        {
            if (Items == null || Items.Count() == 0)
            {
                if (EmptyDataTemplate != null)
                    EmptyDataTemplate.InstantiateIn(this);
            }
            else if (GenerateMethod != null)
            {
                int i = 0;
                foreach (object n in Items)
                    Controls.Add(GenerateMethod(i++, n));
            }
            ChildControlsCreated = true;
        }

        #region IRepeater 成员

        public void SetItems(IEnumerable Items)
        {
            this.Items = Items;
        }

        #endregion
    }
}
