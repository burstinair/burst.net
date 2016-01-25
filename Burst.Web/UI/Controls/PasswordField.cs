using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Burst.Web.UI.Controls
{
    public delegate string EncryptDelegate(string UserName, string Password);

    public class PasswordField : FieldBase
    {
        public string FieldName { get; set; }
        public string UserNameFieldName { get; set; }

        protected TextBox value_box;

        protected override void CreateChildControls()
        {
            var label = new HtmlGenericControl("span");
            label.InnerText = Title;
            this.Controls.Add(label);

            value_box = new TextBox();
            (value_box as TextBox).CssClass = "text";
            this.Controls.Add(value_box);

            ChildControlsCreated = true;
        }

        public EncryptDelegate EncryptMethod { get; set; }

        public override void Update(IFieldWritable Item)
        {
            if (!string.IsNullOrEmpty(value_box.Text) && EncryptMethod != null)
            {
                Item.SetFieldValue(FieldName, EncryptMethod(this.Item.GetFieldValue(UserNameFieldName) as string, value_box.Text));
            }
        }

        public override IEnumerable<string> UpdateFields
        {
            get { return new string[] { FieldName }; }
        }
    }
}
