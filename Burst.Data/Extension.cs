using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data.Common;
using Burst.Data.Entity;

namespace Burst.Data
{
    public static class Extension
    {
        public static int ExecuteNonQuery(this IEnumerable<Command> cmds, Transaction trans)
        {
            Debug.Assert(cmds != null, "IEnumerable<Command>.ExecuteNonQuery: cmds Cannot be NULL");
            if (cmds == null)
                return -1;
            DbConnection conn = null;
            DbTransaction _trans = null;
            Command _cmd = null;
            try
            {
                if (trans != null)
                {
                    conn = trans.CurrentConnection.OriConnection;
                    _trans = trans.CurrentTransaction;
                }
                else
                {
                    conn = DbProvider.Current.CreateConnection();
                    conn.Open();
                    _trans = conn.BeginTransaction();
                }
                int res = 0;
                foreach (Command cmd in cmds)
                {
                    _cmd = cmd;
                    res = cmd.AsCommonCommand(conn, _trans).ExecuteNonQuery();
                }
                if (trans == null)
                    _trans.Commit();
                return res;
            }
            catch (Exception e)
            {
                #if DEBUG
                CommandException ce = new CommandException("ExecuteNonQuery", _cmd, e);
                Debug.WriteLine(ce);
                throw ce;
                #endif
                try
                {
                    if (trans == null)
                        trans.Rollback();
                    else
                        trans.success = false;
                }
                catch (Exception ex)
                {
                    #if DEBUG
                    CommandException ce2 = new CommandException("ExecuteNonQuery", _cmd, ex);
                    Debug.WriteLine(ce2);
                    throw ce2;
                    #endif
                }
                return -1;
            }
            finally
            {
                try
                {
                    if (trans == null)
                        conn.Close();
                }
                catch (Exception e)
                {
                    #if DEBUG
                    CommandException ce = new CommandException("ExecuteNonQuery", _cmd, e);
                    Debug.WriteLine(ce);
                    throw ce;
                    #endif
                }
            }
        }
        public static int ExecuteNonQuery(this IEnumerable<Command> cmds)
        {
            return ExecuteNonQuery(cmds, null);
        }
    }
}
