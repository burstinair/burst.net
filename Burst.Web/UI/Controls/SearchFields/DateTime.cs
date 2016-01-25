using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Burst.Data.CommandBuilder;

namespace Burst.Web.UI.Controls.SearchFields
{
    public class DateTime : SearchField
    {
        public override CompareType Type
        {
            get { return CompareType.Between; }
        }

        public override Compare Compare
        {
            get
            {
                EnsureChildControls();
                System.DateTime min;
                System.DateTime.TryParse(MinValueBox.Text, out min);
                System.DateTime max;
                System.DateTime.TryParse(MaxValueBox.Text, out max);
                return new Compare(Type, Field, min, max);
            }
        }

        public TextBox MinValueBox { get; set; }
        public TextBox MaxValueBox { get; set; }

        protected override void CreateChildControls()
        {
            MinValueBox = new TextBox();
            Controls.Add(MinValueBox);
            MaxValueBox = new TextBox();
            Controls.Add(MaxValueBox);
            ChildControlsCreated = true;
        }
    }
}
