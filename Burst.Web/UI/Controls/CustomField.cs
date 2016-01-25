using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Burst.Web.UI.Controls
{
    public class CustomField : FieldBase, INamingContainer
    {
        public UpdateEventHandler UpdateMethod { get; set; }
        public CreateChildrenEventHandler CreateChildrenMethod { get; set; }

        protected override void CreateChildControls()
        {
            if (CreateChildrenMethod != null)
                CreateChildrenMethod(this, new CreateChildrenEventArgs(Item, this));
            ChildControlsCreated = true;
        }

        public override void Update(IFieldWritable Item)
        {
            if (UpdateMethod != null)
                UpdateMethod(this, new UpdateEventArgs(Item));
        }

        private IEnumerable<string> _update_fields = new string[] { };
        public override IEnumerable<string> UpdateFields
        {
            get { return _update_fields; }
        }
        public void SetUpdateFields(IEnumerable<string> value)
        {
            _update_fields = value;
        }
    }
}
