using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using Burst.Data.CommandBuilder;
using Burst.Data.Schema;

namespace Burst.Data.MySQL
{
    public class MySQLAdapter : IDbAdapter
    {
        public virtual void InitializeOptions(NameValueList options)
        {
            options["DefaultDbProviderFactory"] = "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Version=6.3.6.0, Culture=neutral, PublicKeyToken=C5687FC88969C44D";
        }

        public virtual string LikePattern(string oriValue)
        {
            return oriValue.Replace("\\", "\\\\").Replace("_", "\\_").Replace("%", "\\%");
        }

        public virtual string LastInsertId()
        {
            return "last_insert_id()";
        }

        public virtual string Concat(params string[] ss)
        {
            StringBuilder res = new StringBuilder("concat(");
            foreach (string s in ss)
                res.AppendFormat("{0},", s);
            if (ss.Length > 0)
                res.Remove(res.Length - 1, 1);
            return res + ")";
        }

        public virtual string EnsureIdentifier(string ori)
        {
            return string.Format("`{0}`", ori);
        }

        /// <summary>
        /// 所需参数: Host, Database, Username, Password。
        /// </summary>
        public virtual Dictionary<string, string> ConnectionStringRequestFields
        {
            get
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                res.Add("Host", "Host");
                res.Add("Database", "Database");
                res.Add("Username", "String");
                res.Add("Password", "String");
                res.Add("Charset", "String");
                return res;
            }
        }

        /// <summary>
        /// 所需参数: Host, Database, Username, Password, ExtraConfiguration(NameValueList)。
        /// </summary>
        public virtual string BuildConnectionString(NameValueList pms)
        {
            string res = "";
            if (pms.ContainsKey("Host"))
                if (!string.IsNullOrEmpty(pms["Host"] as string))
                    res = string.Format("Server={0};", pms["Host"]);
            if (pms.ContainsKey("Database"))
                if (!string.IsNullOrEmpty(pms["Database"] as string))
                    res = string.Format("{0}Database={1};", res, pms["Database"]);
            if (pms.ContainsKey("Username"))
                if (!string.IsNullOrEmpty(pms["Username"] as string))
                    res = string.Format("{0}Uid={1};", res, pms["Username"]);
            if (pms.ContainsKey("Password"))
                if (!string.IsNullOrEmpty(pms["Password"] as string))
                    res = string.Format("{0}Pwd={1};", res, pms["Password"]);
            if (pms.ContainsKey("Charset"))
                if (!string.IsNullOrEmpty(pms["Charset"] as string))
                    res = string.Format("{0}Charset={1};", res, pms["Charset"]);
            if (pms.ContainsKey("ExtraConfiguration"))
                if (pms["ExtraConfiguration"] is NameValueList)
                    foreach (NameValue nv in pms["ExtraConfiguration"] as NameValueList)
                        res = string.Format("{0}{1}={2};", res, nv.Name, nv.Value);
            return res;
        }

        public virtual void AppendPage(Command cmd, string keyFieldName, string tableName, Page page, Where where)
        {
            cmd.Append(" limit @0PageStartIndex, @0PageCount");
            cmd.AddParameter("@0PageStartIndex", page.StartPos);
            cmd.AddParameter("@0PageCount", page.Count);
        }

        public virtual Command GetReplaceCommand(string tableName, object key, string keyFieldName, NameValueList pms, DbProvider provider)
        {
            StringBuilder vs = new StringBuilder();
            foreach (string s in pms.Keys)
                vs.AppendFormat("{0},", s);
            if (vs.Length > 0)
                vs.Remove(vs.Length - 1, 1);
            string cmd = string.Format("replace into {0} values ({1})", tableName, vs);
            return provider.CreateCommand(cmd, pms);
        }

        public virtual DbSchema GetSchema(DbConnection Connection)
        {
            var res = new MySQLSchema();
            res.LoadSchema(Connection);
            return res;
        }
    }
}
