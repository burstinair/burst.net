using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Burst.Json
{
    public class JsonObjectParser : JsonObjectParserBase
    {
        protected override Type GetFieldType(string field, object res, Type type)
        {
            return (res as IFieldWritable).GetFieldType(field);
        }

        protected override void SetFieldValue(string field, object value, object res, Type type)
        {
            (res as IFieldWritable).SetFieldValue(field, value);
        }

        protected override object CreateResult(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}