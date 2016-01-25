using System;
using System.Collections.Generic;
using System.Text;

namespace Burst.Json
{
    public class JsonSerializer : ISerializer
    {
        public object Serialize(object oriobj)
        {
            return JsonUtils.Serialize(oriobj);
        }

        public object Deserialize(object data, Type type)
        {
            return JsonUtils.ParseAs(data.ToString(), type);
        }
    }
}
