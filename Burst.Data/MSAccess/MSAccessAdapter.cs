using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Data.Common;
using Burst.Data.Entity;
using Burst.Data.MSSQL;
using Burst.Data.Schema;

namespace Burst.Data.MSAccess
{
    public class MSAccessAdapter : MSSQLAdapter
    {
        public override void InitializeOptions(NameValueList options)
        {
            options["DefaultDbProviderFactory"] = System.Data.OleDb.OleDbFactory.Instance;
        }

        public override string BuildConnectionString(NameValueList pms)
        {
            StringBuilder res = new StringBuilder("Provider=Microsoft.Jet.OLEDB.4.0;");
            if (pms.ContainsKey("FilePath"))
                if (!string.IsNullOrEmpty(pms["FilePath"] as string))
                    res.AppendFormat("Data Source={0};", Path.GetFullPath(pms["FilePath"] as string));
            if (pms.ContainsKey("Password"))
                if (!string.IsNullOrEmpty(pms["Password"] as string))
                    res.AppendFormat("Password={0};", pms["Password"]);
            if (pms.ContainsKey("ExtraConfiguration"))
                if (pms["ExtraConfiguration"] is NameValueList)
                    foreach (NameValue nv in pms["ExtraConfiguration"] as NameValueList)
                        res.AppendFormat("{0}={1};", nv.Name, nv.Value);
            return res.ToString();
        }

        public override DbSchema GetSchema(DbConnection Connection)
        {
            var res = new MSAccessSchema();
            res.LoadSchema(Connection);
            return res;
        }
    }
}
