using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Burst.Web.UI
{
    public delegate void CreateChildrenEventHandler(object sender, CreateChildrenEventArgs e);
    public class CreateChildrenEventArgs : EventArgs
    {
        public IFieldReadable Item { get; set; }
        public Control Control { get; set; }

        public CreateChildrenEventArgs()
        { }
        public CreateChildrenEventArgs(IFieldReadable Item, Control Control)
        {
            this.Item = Item;
            this.Control = Control;
        }
    }

    public delegate void UpdateEventHandler(object sender, UpdateEventArgs e);
    public class UpdateEventArgs : EventArgs
    {
        public IFieldWritable Item { get; set; }

        public UpdateEventArgs()
        { }
        public UpdateEventArgs(IFieldWritable Item)
        {
            this.Item = Item;
        }
    }
}
