using System;

namespace Burst.Json
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class JsonIgnoreAttribute : Attribute
    {
        public JsonIgnoreAttribute()
        {
        }
    }
}
