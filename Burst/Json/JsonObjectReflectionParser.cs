using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Burst.Json
{
    public class JsonObjectReflectionParser : JsonObjectParserBase
    {
        protected override Type GetFieldType(string field, object res, Type targetType)
        {
            MemberInfo curMemberInfo = targetType.GetFieldOrProperty(field);
            if (curMemberInfo != null)
                return curMemberInfo.GetFieldOrPropertyType();
            else
                throw new JsonParseException("The Object does not contains a field named {0}.", field);
        }

        protected override void SetFieldValue(string field, object value, object res, Type targetType)
        {
            MemberInfo curMemberInfo = targetType.GetFieldOrProperty(field);
            if (curMemberInfo != null)
                curMemberInfo.SetValue(res, value);
            else
                throw new JsonParseException("The Object does not contains a field named {0}.", field);
        }

        protected override object CreateResult(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}