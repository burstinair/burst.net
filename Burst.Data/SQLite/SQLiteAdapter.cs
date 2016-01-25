using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Data;
using System.Data.Common;
using Burst.Data.Entity;
using Burst.Data.MySQL;

namespace Burst.Data.SQLite
{
    public class SQLiteAdapter : MySQLAdapter
    {
        public override void InitializeOptions(NameValueList options)
        {
            options["DefaultDbProviderFactory"] = "System.Data.SQLite.SQLiteFactory, System.Data.SQLite, Version=1.0.76.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139";
        }

        public override string LastInsertId()
        {
            return "last_insert_rowid()";
        }

        public override string Concat(params string[] ss)
        {
            StringBuilder res = new StringBuilder("");
            foreach (string s in ss)
                res.AppendFormat("{0}||", s);
            if (ss.Length > 0)
                res.Remove(res.Length - 2, 2);
            return res.ToString();
        }

        public override string EnsureIdentifier(string ori)
        {
            return string.Format("[{0}]", ori);
        }

        /// <summary>
        /// 所需参数: FilePath, Version(2, 3), Password, ReadOnly(true, false)。
        /// </summary>
        public override Dictionary<string, string> ConnectionStringRequestFields
        {
            get
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                res.Add("FilePath", "File");
                res.Add("Version", "2;3");
                res.Add("Password", "String");
                res.Add("ReadOnly", "Boolean");
                return res;
            }
        }

        /// <summary>
        /// 所需参数: FilePath, Version(2, 3), Password, ReadOnly(true, false), ExtraConfiguration(NameValueList)。
        /// </summary>
        public override string BuildConnectionString(NameValueList pms)
        {
            StringBuilder res = new StringBuilder();
            if (pms.ContainsKey("FilePath"))
                if (!string.IsNullOrEmpty(pms["FilePath"] as string))
                    res.AppendFormat("Data Source={0};", Path.GetFullPath(pms["FilePath"] as string));
            if (pms.ContainsKey("Version"))
                if (!string.IsNullOrEmpty(pms["Version"] as string))
                    res.AppendFormat("Version={0};", pms["Version"]);
            if (pms.ContainsKey("Password"))
                if (!string.IsNullOrEmpty(pms["Password"] as string))
                    res.AppendFormat("Password={0};", pms["Password"]);
            if (pms.ContainsKey("ReadOnly"))
                if (!string.IsNullOrEmpty(pms["ReadOnly"] as string))
                    res.AppendFormat("Read Only={0};", pms["ReadOnly"]);
            if (pms.ContainsKey("ExtraConfiguration"))
                if (pms["ExtraConfiguration"] is NameValueList)
                    foreach (NameValue nv in pms["ExtraConfiguration"] as NameValueList)
                        res.AppendFormat("{0}={1};", nv.Name, nv.Value);
            return res.ToString();
        }

        public override Schema.DbSchema GetSchema(DbConnection Connection)
        {
            var res = new SQLiteSchema();
            res.LoadSchema(Connection);
            return res;
        }
    }
}
