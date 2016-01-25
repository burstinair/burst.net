using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Burst.Json
{
    internal enum JsonType
    {
        None, IJsonObject, IDictionary, IList, String, Object, ValueType, Array
    }

    /// <summary>
    /// 提供Json编码、解析的一般方法。
    /// </summary>
    public static class JsonUtils
    {
        internal const char DoubleQuote = '\"';
        internal const char SingleQuote = '\'';
        internal const char Underline = '_';
        internal const char Colon = ':';
        internal const char Comma = ',';
        internal const char LeftBrace = '{';
        internal const char RightBrace = '}';
        internal const char LeftBracket = '[';
        internal const char RightBracket = ']';
        internal const char BackSlash = '\\';
        internal const char Enter = '\r';
        internal const char HalfEnter = 'r';
        internal const char Wrap = '\n';
        internal const char HalfWrap = 'n';
        internal const char Table = '\t';
        internal const char HalfTable = 't';
        internal const string True = "true";
        internal const string False = "false";
        internal const char Point = '.';

        internal static JsonObjectParser JsonObjectParser = new JsonObjectParser();
        internal static JsonDictionaryParser JsonDictionaryParser = new JsonDictionaryParser();
        internal static JsonObjectReflectionParser JsonObjectReflectionParser = new JsonObjectReflectionParser();
        internal static Type ObjectType = typeof(object);

        /// <summary>
        /// 将源字符串中的CR、LF、Tab和引号进行转移字符(\)编码。
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>编码后的字符串</returns>
        public static string EncodeCRLFTabQuote(string source)
        {
            if (source == null)
                return null;
            StringBuilder res = new StringBuilder();
            int p = 0;
            while (p < source.Length)
            {
                switch (source[p])
                {
                    case '\\':
                        res.Append("\\\\");
                        p++;
                        break;
                    case '\n':
                        res.Append("\\n");
                        if (p + 1 < source.Length)
                            if (source[p + 1] == '\r')
                            {
                                p += 2;
                                continue;
                            }
                        p++;
                        break;
                    case '\r':
                        res.Append("\\n");
                        if (p + 1 < source.Length)
                            if (source[p + 1] == '\n')
                            {
                                p += 2;
                                continue;
                            }
                        p++;
                        break;
                    case '\t':
                        res.Append("\\t");
                        p++;
                        break;
                    case '\'':
                        res.Append("\\'");
                        p++;
                        break;
                    case '\"':
                        res.Append("\\\"");
                        p++;
                        break;
                    default:
                        res.Append(source[p]);
                        p++;
                        break;
                }
            }
            return res.ToString();
        }

        /// <summary>
        /// 将指定的对象编码为Json字符串。
        /// </summary>
        /// <param name="s">指定的对象</param>
        /// <returns>编码后的字符串</returns>
        public static string Serialize(object s)
        {
            return Serialize(s, null);
        }

        /// <summary>
        /// 将指定对象中具有指定JsonGroupAttribute的字段编码为Json字符串。
        /// </summary>
        /// <param name="s">指定的对象</param>
        /// <param name="group">指定JsonGroupAttribute的名称</param>
        /// <returns>编码后的字符串</returns>
        public static string Serialize(object s, string group)
        {
            if (s == null || s == DBNull.Value)
                return "\"\"";
            else if (s is IJsonSerializeObject)
                return (s as IJsonSerializeObject).SerializeToJsonString();
            else if (s is IFieldViewable && s is IFieldReadable)
            {
                StringBuilder res = new StringBuilder("{");
                foreach (IFieldInfo i in (s as IFieldViewable).Fields)
                    res.Append(string.Format(@"""{0}"":{1},", i.Name, Serialize((s as IFieldReadable).GetFieldValue(i.Name))));
                if (res.Length > 1)
                    res.Remove(res.Length - 1, 1);
                return res + "}";
            }
            else if (s is ValueType || s.GetType().IsPlainValue())
                return string.Format("\"{0}\"",
                    EncodeCRLFTabQuote(Utils.SerializeToString(s, SerializeType.None))
                );
            else if (s is IDictionary)
            {
                StringBuilder res = new StringBuilder("{");
                foreach (DictionaryEntry i in s as IDictionary)
                    res.Append(string.Format(@"""{0}"":{1},", i.Key, Serialize(i.Value)));
                if (res.Length > 1)
                    res.Remove(res.Length - 1, 1);
                return res + "}";
            }
            else if (s is IEnumerable)
            {
                StringBuilder res = new StringBuilder("[");
                foreach (var i in s as IEnumerable)
                    res.Append(string.Format("{0},", Serialize(i)));
                if (res.Length > 1)
                    res.Remove(res.Length - 1, 1);
                return res + "]";
            }
            else
            {
                StringBuilder res = new StringBuilder("{");
                Type t = s.GetType();
                foreach (MemberInfo mi in t.GetFieldsAndProperties())
                    if (mi.GetAttribute<JsonIgnoreAttribute>() == null)
                    {
                        if (group != null)
                        {
                            JsonGroupAttribute jga = mi.GetAttribute<JsonGroupAttribute>();
                            if (jga != null)
                                if (jga.Group == group)
                                    res.Append(string.Format(@"""{0}"":{1},", mi.Name, Serialize(mi.GetValue(s))));
                        }
                        else
                            res.Append(string.Format(@"""{0}"":{1},", mi.Name, Serialize(mi.GetValue(s))));
                    }
                if (res.Length > 1)
                    res.Remove(res.Length - 1, 1);
                return res + "}";
            }
        }
        
        internal static int ParseJsonArray(string jsonstr, int offset, IList res, Type elementType)
        {
            int i = offset, state = 0;
            while (i < jsonstr.Length - 1)
            {
                i++;
                if (jsonstr[i] == RightBracket)
                    return i + 1;
                else if (state == 0)
                {
                    if (char.IsWhiteSpace(jsonstr, i))
                        continue;
                    res.Add(ParseAs(jsonstr, elementType, i, out i));
                    i--;
                    state = 1;
                }
                else if (state == 1)
                {
                    if (char.IsWhiteSpace(jsonstr, i))
                        continue;
                    if (jsonstr[i] == Comma)
                        state = 0;
                    else
                        throw new JsonParseException("Json Array Parse Error at {0}.", i);
                }
            }
            throw new JsonParseException("Json Array Parse Error at {0}.", i);
        }
        internal static object ParseAs(string jsonstr, Type ast, int offset, out int endIndex)
        {
            endIndex = -1;
            JsonType res_type = JsonType.None;
            if (ast.Implements(typeof(IFieldWritable)))
                res_type = JsonType.IJsonObject;
            else if (ast.Implements(typeof(IDictionary)))
                res_type = JsonType.IDictionary;
            else if (ast.IsArray)
                res_type = JsonType.Array;
            else if (ast.Implements(typeof(IList)))
                res_type = JsonType.IList;
            else if (TypeUtils.IsSame<object>(ast))
                res_type = JsonType.Object;
            else if (TypeUtils.IsSame<string>(ast))
                res_type = JsonType.String;
            else if (TypeUtils.IsSubclassOf<ValueType>(ast) || ast.IsPlainValue())
                res_type = JsonType.ValueType;

            object value = null;
            if (jsonstr[offset] == LeftBrace)
            {
                JsonObjectParserBase parser;
                if (res_type == JsonType.IJsonObject)
                    parser = JsonObjectParser;
                else if(res_type == JsonType.IDictionary || res_type == JsonType.Object)
                    parser = JsonDictionaryParser;
                else
                    parser = JsonObjectReflectionParser;
                endIndex = parser.ParseJsonObject(jsonstr, offset, out value, ast);
            }
            else if (jsonstr[offset] == LeftBracket)
            {
                Type elementType;
                if (res_type == JsonType.IList)
                {
                    value = Activator.CreateInstance(ast);
                    if (ast.IsGenericType)
                        elementType = ast.GetGenericArguments()[0];
                    else
                        elementType = ObjectType;
                }
                else if (res_type == JsonType.Array)
                {
                    value = new List<object>();
                    if (ast.GetArrayRank() == 1)
                        elementType = ast.GetElementType();
                    else
                        throw new JsonParseException("Array Type Rank does not match JsonArray require.");
                }
                else if (res_type == JsonType.Object)
                {
                    value = new List<object>();
                    elementType = ObjectType;
                }
                else
                    throw new JsonParseException("Target Type does not match JsonArray require.");
                endIndex = ParseJsonArray(jsonstr, offset, value as IList, elementType);
            }
            else if (jsonstr[offset] == DoubleQuote || jsonstr[offset] == SingleQuote)
            {
                StringBuilder res = new StringBuilder();
                for (int i = offset + 1; i < jsonstr.Length; i++)
                {
                    if (jsonstr[i] == BackSlash)
                    {
                        var signal = jsonstr[i + 1];
                        switch (signal)
                        {
                            case HalfWrap:
                                res.Append(Wrap);
                                break;
                            case HalfEnter:
                                res.Append(Enter);
                                break;
                            case HalfTable:
                                res.Append(Table);
                                break;
                            default:
                                res.Append(signal);
                                break;
                        }
                        i++;
                    }
                    else if (jsonstr[i] == jsonstr[offset])
                    {
                        endIndex = i + 1;
                        value = res.ToString();
                        break;
                    }
                    else
                        res.Append(jsonstr[i]);
                }
            }
            if (endIndex == -1 && offset + 4 <= jsonstr.Length)
                if (string.Equals(jsonstr.Substring(offset, 4), True, StringComparison.CurrentCultureIgnoreCase))
                {
                    value = true;
                    endIndex = offset + 4;
                }
            if (endIndex == -1 && offset + 5 <= jsonstr.Length)
                if (string.Equals(jsonstr.Substring(offset, 5), False, StringComparison.CurrentCultureIgnoreCase))
                {
                    value = false;
                    endIndex = offset + 5;
                }
            if(endIndex == -1)
            {
                bool isFloat = false;
                for (int j = offset; j < jsonstr.Length; j++)
                {
                    if (jsonstr[j] == Point)
                        if (isFloat)
                            throw new JsonParseException("Number Parse Error at {0}({1}).", offset, jsonstr.Substring(offset, j - offset));
                        else
                            isFloat = true;
                    else if (!char.IsDigit(jsonstr[j]))
                    {
                        try
                        {
                            if (isFloat)
                                value = double.Parse(jsonstr.Substring(offset, j - offset));
                            else
                                value = int.Parse(jsonstr.Substring(offset, j - offset));
                        }
                        catch
                        {
                            throw new JsonParseException("Number Parse Error at {0}({1}).", offset, jsonstr.Substring(offset, j - offset));
                        }
                        endIndex = j;
                        break;
                    }
                }
            }
            if (endIndex == -1)
                throw new JsonParseException("Json Parse Error at {0}.", offset);
            if (res_type == JsonType.IJsonObject || res_type == JsonType.Object ||
                res_type == JsonType.IDictionary || res_type == JsonType.IList)
                return value;
            else if (res_type == JsonType.Array)
                return (value as List<object>).ToArray();
            else if (res_type == JsonType.String)
                if (value is string || value is ValueType)
                    return value.ToString();
                else
                    return jsonstr.Substring(offset, endIndex - offset);
            else if (res_type == JsonType.ValueType)
            {
                if (value is string)
                    return Utils.DeserializeAs(value, ast, SerializeType.None);
                else if (value is ValueType)
                    return value;
            }
            throw new JsonParseException("Json Parse Error at {0}.", offset);
        }
        
        /// <summary>
        /// 将指定的Json字符串解析为制定类型的对象，若指定的类型为Object，则解析为Dictionary<string, object>。
        /// </summary>
        /// <typeparam name="T">指定的类型</typeparam>
        /// <param name="jsonstr">要解析的Json字符串</param>
        /// <returns>解析后的对象</returns>
        public static T ParseAs<T>(string jsonstr)
        {
            if (string.IsNullOrEmpty(jsonstr))
                return default(T);
            object res = null;
            try
            {
                int _t;
                res = ParseAs(jsonstr.Trim(), typeof(T), 0, out _t);
                return (T)res;
            }
            catch (InvalidCastException) { }
            throw new JsonParseException("Json Cast Error, TargetType: {0}, Source: {1}.", typeof(T), res);
        }

        /// <summary>
        /// 将指定的Json字符串解析为制定类型的对象，若指定的类型为Object，则解析为Dictionary<string, object>。
        /// </summary>
        /// <param name="jsonstr">要解析的Json字符串</param>
        /// <param name="t">指定的类型</param>
        /// <returns>解析后的对象</returns>
        public static object ParseAs(string jsonstr, Type t)
        {
            if (string.IsNullOrEmpty(jsonstr))
                return null;
            int _t;
            return ParseAs(jsonstr.Trim(), t, 0, out _t);
        }
    }
}
