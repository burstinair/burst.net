using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Burst.Data.Entity;

namespace Burst.Web.UI.Controls
{
    [ToolboxData("<{0}:Form runat=\"server\"></{0}:Form>")]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    [ParseChildren(true, "Children"), PersistChildren(false)]
    public class Form : CompositeControl, INamingContainer
    {
        public Form()
        {
            this.Tag = HtmlTextWriterTag.Div;
            this.ItemTag = HtmlTextWriterTag.Div;

            this.SuccessTip = "操作成功。";
            this.SuccessCssClass = "success";
            this.FailCssClass = "操作失败。";
            this.FailCssClass = "fail";
        }
        protected string[] UpdateFields;

        [Browsable(false)]
        public IDataEntity DataEntity
        {
            get { return ViewState["DataEntity"] as IDataEntity; }
            set
            {
                ViewState["DataEntity"] = value;
                RecreateChildControls();
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get { return Tag; }
        }

        public string DataEntityTypeName { get; set; }
        protected Type DataEntityType
        {
            get
            {
                if (ViewState["DataEntityType"] == null)
                    ViewState["DataEntityType"] = Type.GetType(DataEntityTypeName);
                return ViewState["DataEntityType"] as Type;
            }
        }

        public HtmlTextWriterTag Tag { get; set; }
        public HtmlTextWriterTag ItemTag { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Page != null)
                Page.VerifyRenderingInServerForm(this);
            base.Render(writer);
        }

        protected Label Tip;

        public event CreateChildrenEventHandler CreatingChildren;

        protected override void CreateChildControls()
        {
            var entity = DataEntity;
            if (entity == null)
                entity = Activator.CreateInstance(DataEntityType) as IDataEntity;

            if (CreatingChildren != null)
                CreatingChildren(this, new CreateChildrenEventArgs(entity, null));

            Tip = new Label();
            this.Controls.Add(Tip);

            var res = new List<string>();
            foreach (var i in Children)
            {
                if (i is FieldBase)
                {
                    (i as FieldBase).Initialize(entity, ItemTag);
                    res.AddRange((i as FieldBase).UpdateFields);
                }
                else if (i is SubmitButton)
                    (i as SubmitButton).Click += new EventHandler(_submit);

                this.Controls.Add(i);
            }
            this.UpdateFields = res.Distinct().ToArray();
            ChildControlsCreated = true;
        }

        public event UpdateEventHandler Updating;

        void _submit(object sender, EventArgs e)
        {
            bool Insert = false;
            if (DataEntity == null)
            {
                Insert = true;
                DataEntity = Activator.CreateInstance(DataEntityType) as IDataEntity;
            }

            foreach (var i in Children)
                if (i is FieldBase)
                    (i as FieldBase).Update(DataEntity);

            if (Updating != null)
                Updating(this, new UpdateEventArgs(DataEntity));

            try
            {
                int res;
                if (Insert)
                    res = DataEntity.Insert(Data.CommandBuilder.InsertType.New, null);
                else
                    res = DataEntity.Update(null, UpdateFields);
                if (res > -1)
                {
                    Tip.Text = SuccessTip;
                    Tip.CssClass = FailCssClass;
                }
                else
                {
                    Tip.Text = FailTip;
                    Tip.CssClass = FailCssClass;
                }
            }
            catch
            {
                Tip.Text = FailTip;
                Tip.CssClass = FailCssClass;
            }
        }

        public string SuccessTip { get; set; }
        public string FailTip { get; set; }
        public string SuccessCssClass { get; set; }
        public string FailCssClass { get; set; }

        [Browsable(false)]
        public List<Control> Children { get; set; }
    }
}
