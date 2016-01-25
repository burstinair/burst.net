using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.Common;
using System.Data;
using Burst.Data.CommandBuilder;
using Burst.Data.Schema;

namespace Burst.Data.MSSQL
{
    public class MSSQLAdapter : IDbAdapter
    {
        public virtual void InitializeOptions(NameValueList options)
        {
            options["DefaultDbProviderFactory"] = System.Data.SqlClient.SqlClientFactory.Instance;
        }

        public virtual string LastInsertId()
        {
            return "last_insert_id()";
        }

        public virtual string LikePattern(string oriValue)
        {
            return oriValue.Replace("\\", "\\\\").Replace("_", "\\_").Replace("%", "\\%");
        }

        public virtual string Concat(params string[] ss)
        {
            StringBuilder res = new StringBuilder();
            foreach (string s in ss)
                res.AppendFormat("{0}+", s);
            if (ss.Length > 0)
                res.Remove(res.Length - 1, 1);
            return res.ToString();
        }

        public virtual string EnsureIdentifier(string ori)
        {
            return string.Format("[{0}]", ori);
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
                return res;
            }
        }

        private NameValueList _pms = new NameValueList();

        /// <summary>
        /// 所需参数: Host, Database, Username, Password, ExtraConfiguration(NameValueList)。
        /// </summary>
        public virtual string BuildConnectionString(NameValueList pms)
        {
            string res = "";
            if (pms.ContainsKey("Host"))
                if (!string.IsNullOrEmpty(pms["Host"] as string))
                    res = string.Format("Data Source={0};", pms["Host"]);
            if (pms.ContainsKey("Database"))
                if (!string.IsNullOrEmpty(pms["Database"] as string))
                    res = string.Format("{0}Initial Catalog={1};", res, pms["Database"]);
            if (pms.ContainsKey("Username"))
                if (!string.IsNullOrEmpty(pms["Username"] as string))
                    res = string.Format("{0}User ID={1};", res, pms["Username"]);
            if (pms.ContainsKey("Password"))
                if (!string.IsNullOrEmpty(pms["Password"] as string))
                    res = string.Format("{0}Password={1};", res, pms["Password"]);
            if (pms.ContainsKey("ExtraConfiguration"))
                if (pms["ExtraConfiguration"] is NameValueList)
                    foreach (NameValue nv in pms["ExtraConfiguration"] as NameValueList)
                        res = string.Format("{0}{1}={2};", res, nv.Name, nv.Value);
            return res;
        }

        public virtual void AppendPage(Command cmd, string keyFieldName, string tableName, Page page, Where where)
        {
            if (page == null)
                return;

            string ori = cmd.QueryString;
            if (ori.Substring(0, 7).ToLower() != "select ")
                return;
            string oriselect = ori.Substring(7, ori.IndexOf(" from ", StringComparison.CurrentCultureIgnoreCase) - 7);

            int where_index = ori.LastIndexOf(" where ", StringComparison.CurrentCultureIgnoreCase);
            if (where_index != -1)
                ori = ori.Substring(where_index + 7);

            cmd.QueryString = string.Format(
                "select top {0} {1} from [{2}] where {6}([{3}] not in (select top {4} [{3}] from [{2}]{5}))",
                page.Count, oriselect, tableName, keyFieldName, page.StartPos,
                where_index == -1 ? string.Empty : " where " + ori,
                where_index == -1 ? string.Empty : ori + " and "
            );

            if (where != null)
                Builder.AppendOrders(cmd, where.Orders, this);
        }

        public virtual Command GetReplaceCommand(string tableName, object key, string keyFieldName, NameValueList pms, DbProvider provider)
        {
            return null;
        }

        public virtual DbSchema GetSchema(DbConnection Connection)
        {
            var res = new MSSQLSchema();
            res.LoadSchema(Connection);
            return res;
        }
    }
}
