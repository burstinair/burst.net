using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Burst.Json
{
    public class JsonDictionaryParser : JsonObjectParserBase
    {
        protected override Type GetFieldType(string field, object res, Type dic_type)
        {
            if (dic_type.IsGenericType)
            {
                Type[] rtt = dic_type.GetGenericArguments();
                if (rtt.Length == 2)
                    return rtt[1];
            }
            return JsonUtils.ObjectType;
        }

        protected override void SetFieldValue(string field, object value, object res, Type dic_type)
        {
            if ((res as IDictionary).Contains(field))
                (res as IDictionary)[field] = value;
            (res as IDictionary).Add(field, value);
        }

        protected override object CreateResult(Type type)
        {
            if (type == JsonUtils.ObjectType)
                return new Dictionary<string, object>();
            return Activator.CreateInstance(type);
        }
    }
}