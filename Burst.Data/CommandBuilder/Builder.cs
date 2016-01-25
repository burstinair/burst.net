using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Burst.Data.CommandBuilder
{
    public static class Builder
    {
        public static void AppendSelect(Command cmd, Select select, IDbAdapter adapter)
        {
            if (select == null)
            {
                cmd.Append(" *");
                return;
            }
            switch (select.Type)
            {
                case SelectType.All:
                default:
                    cmd.Append(" *");
                    break;
                case SelectType.Custom:
                    if (select.Fields.Count == 0)
                        return;
                    cmd.Append(" ");
                    foreach (string s in select.Fields)
                        cmd.AppendFormat("{0},", adapter.EnsureIdentifier(s));
                    cmd.RemoveLastChar();
                    break;
            }
        }
        public static void AppendOrders(Command cmd, IEnumerable<Order> orders, IDbAdapter adapter)
        {
            if (orders != null)
                if (orders.Count() > 0)
                {
                    cmd.Append(" order by ");
                    foreach (Order oo in orders)
                        cmd.AppendFormat("{0},", oo.ToString(adapter));
                    cmd.RemoveLastChar();
                }
        }
        public static void AppendWhereByEnsuredKey(Command cmd, Where where, string keyFieldName, IDbAdapter adapter)
        {
            if (where == null)
                return;
            switch (where.Type)
            {
                case WhereType.Key:
                    cmd.AppendFormat(" where {0}=@0WhereKey", keyFieldName);
                    cmd.AddParameter("@0WhereKey", where.SingleObject);
                    break;
                case WhereType.Custom:
                    if (!string.IsNullOrEmpty(where.QueryString))
                    {
                        cmd.AppendFormat(" where {0}", where.QueryString);
                        for (int i = 0; i < where.Parameters.Count; i++)
                            cmd.AddParameter("@" + i, where.Parameters[i]);
                    }
                    break;
            }
            AppendOrders(cmd, where.Orders, adapter);
        }
        public static void AppendWhere(Command cmd, Where where, string keyFieldName, IDbAdapter adapter)
        {
            AppendWhereByEnsuredKey(cmd, where, adapter.EnsureIdentifier(keyFieldName), adapter);
        }
        public static void AppendPage(Command cmd, string keyFieldName, string tableName, Page page, Where where, IDbAdapter adapter)
        {
            if (page == null)
                return;
            adapter.AppendPage(cmd, keyFieldName, tableName, page, where);
        }
    }
}
