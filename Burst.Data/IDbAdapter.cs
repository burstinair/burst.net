using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Burst.Data.CommandBuilder;
using Burst.Data.Schema;

namespace Burst.Data
{
    public interface IDbAdapter
    {
        void InitializeOptions(NameValueList options);
        string LikePattern(string oriValue);
        string Concat(params string[] ss);
        string LastInsertId();
        string EnsureIdentifier(string ori);
        Command GetReplaceCommand(string tableName, object key, string keyFieldName, NameValueList pms, DbProvider provider);
        void AppendPage(Command cmd, string keyFieldName, string tableName, Page page, Where where);

        string BuildConnectionString(NameValueList pms);
        Dictionary<string, string> ConnectionStringRequestFields { get; }
        DbSchema GetSchema(DbConnection Connection);
    }
}
