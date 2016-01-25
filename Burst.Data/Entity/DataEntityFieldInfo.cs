using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Burst.Data.Entity
{
    public class DataEntityFieldInfo : IFieldInfo
    {
        public bool Key;
        public bool Nullable;
        public MemberInfo MemberInfo;
        public DataFieldAttribute Attribute;
        public string EnsuredName;
        public Type Type { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
