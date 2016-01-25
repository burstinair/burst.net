using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Burst.Data.CommandBuilder;

namespace Burst.Web.UI.Controls.SearchFields
{
    public class Like : SearchField
    {
        public override CompareType Type
        {
            get { return CompareType.Like; }
        }

        public override Compare Compare
        {
            get
            {
                EnsureChildControls();
                return new Compare(Type, Field, ValueBox.Text);
            }
        }

        public TextBox ValueBox { get; set; }

        protected override void CreateChildControls()
        {
            ValueBox = new TextBox();
            Controls.Add(ValueBox);
            ChildControlsCreated = true;
        }
    }
}
