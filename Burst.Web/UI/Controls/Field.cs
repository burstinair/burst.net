using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Burst.Web.UI.Controls
{
    public class Field : FieldBase, INamingContainer
    {
        public string FieldName { get; set; }

        public bool ReadOnly { get; set; }
        public string ValueCssClass { get; set; }
        public Field()
        {
            ReadOnly = false;
            ValueCssClass = "value";
        }

        protected Control value_box;
        private object GetValue(Type FieldType)
        {
            object res = null;
            if (value_box is TextBox)
                res = (value_box as TextBox).Text;
            else if (value_box is CheckBox)
                res = (value_box as CheckBox).Checked;
            else if (value_box is DropDownList)
                res = (value_box as DropDownList).SelectedValue;
            return Burst.Utils.DeserializeAs(res, FieldType, SerializeType.None);
        }

        protected T GetOriValue<T>()
        {
            var value = Item.GetFieldValue(FieldName);
            if (value == null)
                return default(T);
            return Burst.Utils.DeserializeAs<T>(value);
        }

        protected override void CreateChildControls()
        {
            var label = new HtmlGenericControl("span");
            label.InnerText = Title;
            this.Controls.Add(label);

            if (ReadOnly)
            {
                value_box = new Label();
                (value_box as Label).CssClass = ValueCssClass;
                (value_box as Label).Text = GetOriValue<string>();
            }
            else
            {
                var type = Item.GetFieldType(FieldName);
                if (type == typeof(Boolean))
                {
                    value_box = new CheckBox();
                    (value_box as CheckBox).Checked = GetOriValue<bool>();
                }
                else if (type.IsEnum)
                {
                    value_box = new DropDownList();
                    List<ListItem> res = new List<ListItem>();
                    List<object> values = new List<object>(Enum.GetValues(type).Cast<object>());
                    foreach (var i in values)
                        res.Add(new ListItem(Enum.GetName(type, i), ((int)i).ToString()));
                    (value_box as DropDownList).DataSource = res;
                    (value_box as DropDownList).DataBind();
                    var value = Item.GetFieldValue(FieldName);
                    if (value != null)
                        (value_box as DropDownList).SelectedIndex = values.IndexOf(Burst.Utils.DeserializeAs(value, type, SerializeType.None));
                    else
                        (value_box as DropDownList).SelectedIndex = 0;
                }
                else
                {
                    value_box = new TextBox();
                    (value_box as TextBox).CssClass = "text";
                    (value_box as TextBox).Text = GetOriValue<string>();
                }
            }
            this.Controls.Add(value_box);

            ChildControlsCreated = true;
        }

        public override void Update(IFieldWritable Item)
        {
            if (!ReadOnly)
                Item.SetFieldValue(FieldName, GetValue(Item.GetFieldType(FieldName)));
        }

        public override IEnumerable<string> UpdateFields
        {
            get
            {
                if (ReadOnly)
                    return new string[] { };
                else
                    return new string[] { FieldName };
            }
        }
    }
}
