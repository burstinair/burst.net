using System;
using System.Collections.Generic;
using System.Text;

namespace Burst.Data.CommandBuilder
{
    public enum SelectType
    {
        All, Custom
    }
    public class Select
    {
        public SelectType Type { get; set; }
        public List<string> Fields { get; set; }

        public Select(SelectType type)
        {
            this.Type = type;
            this.Fields = new List<string>();
        }
        public Select(params string[] customFields)
        {
            this.Type = SelectType.Custom;
            this.Fields = new List<string>(customFields);
        }
    }
}
