using System;

namespace Burst.Json
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class JsonGroupAttribute : Attribute
    {
        public JsonGroupAttribute(string Group)
        {
            this.group = Group;
        }

        protected string group;
        public string Group
        {
            get { return this.group; }
            set { this.group = value; }
        }
    }
}
