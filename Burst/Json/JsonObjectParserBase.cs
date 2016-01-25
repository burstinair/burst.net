using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Burst.Json
{
    public abstract class JsonObjectParserBase
    {
        protected abstract Type GetFieldType(string field, object res, Type type);
        protected abstract void SetFieldValue(string field, object value, object res, Type type);
        protected abstract object CreateResult(Type type);

        internal int ParseJsonObject(string jsonstr, int offset, out object res, Type targetType)
        {
            res = CreateResult(targetType);
            int i = offset, state = 0, cvp = 0;
            string curKey = null;
            while (i < jsonstr.Length - 1)
            {
                i++;
                if (state == 0)
                {
                    if (char.IsWhiteSpace(jsonstr, i))
                        continue;
                    if (jsonstr[i] != JsonUtils.DoubleQuote && jsonstr[i] != JsonUtils.SingleQuote)
                        throw new JsonParseException("Json Object Parse Error at {0}.", i);
                    else
                    {
                        state = 1;
                        cvp = i + 1;
                    }
                }
                else if (state == 1)
                {
                    if (jsonstr[i] == jsonstr[cvp - 1])
                    {
                        if (i == cvp)
                            throw new JsonParseException("Json Object Parse Error at {0}.", i);
                        curKey = jsonstr.Substring(cvp, i - cvp);
                        state = 2;
                    }
                    else if (!char.IsLetterOrDigit(jsonstr[i]) && jsonstr[i] != JsonUtils.Underline)
                        throw new JsonParseException("Json Object Parse Error at {0}.", i);
                }
                else if (state == 2)
                {
                    if (char.IsWhiteSpace(jsonstr, i))
                        continue;
                    if (jsonstr[i] != JsonUtils.Colon)
                        throw new JsonParseException("Json Object Parse Error at {0}.", i);
                    state = 3;
                }
                else if (state == 3)
                {
                    if (char.IsWhiteSpace(jsonstr, i))
                        continue;
                    SetFieldValue(
                        curKey,
                        JsonUtils.ParseAs(
                            jsonstr,
                            GetFieldType(curKey, res, targetType),
                            i, out i
                        ),
                        res, targetType
                    );
                    i--;
                    state = 4;
                }
                else if (state == 4)
                {
                    if (char.IsWhiteSpace(jsonstr, i))
                        continue;
                    if (jsonstr[i] == JsonUtils.RightBrace)
                        return i + 1;
                    if (jsonstr[i] == JsonUtils.Comma)
                        state = 0;
                    else
                        throw new JsonParseException("Json Object Parse Error at {0}.", i);
                }
            }
            throw new JsonParseException("Json Object Parse Error at {0}.", i);
        }
    }
}