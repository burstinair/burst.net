using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.Common;

namespace Burst.Data
{
    public class Command
    {
        protected StringBuilder _cmd;
        public string QueryString
        {
            get { return _cmd.ToString(); }
            set { _cmd = new StringBuilder(value); }
        }

        protected NameValueList _pms;
        public NameValueList Parameters
        {
            get { return _pms; }
        }

        protected DbProvider _dbprovider;
        public DbProvider DbProvider
        {
            get { return _dbprovider; }
            internal set { _dbprovider = value; }
        }

        public Command(string cmd, params object[] pms)
        {
            if (string.IsNullOrEmpty(cmd))
                cmd = "";
            this._cmd = new StringBuilder(cmd);
            this._pms = new NameValueList();
            if (pms != null)
                for (int i = 0; i < pms.Length; i++)
                    this._pms.Add("@" + i, pms[i]);
            this._dbprovider = Burst.Data.DbProvider.Current;
        }
        public Command(string cmd, NameValueList pms)
        {
            if (string.IsNullOrEmpty(cmd))
                cmd = "";
            this._cmd = new StringBuilder(cmd);
            this._pms = new NameValueList();
            if (pms != null)
                foreach (NameValue i in pms)
                    this._pms.Add(i.Name, i.Value);
            this._dbprovider = Burst.Data.DbProvider.Current;
        }

        public void Append(string s)
        {
            _cmd.Append(s);
        }
        public void AppendFormat(string format, params object[] pms)
        {
            _cmd.AppendFormat(format, pms);
        }
        public void RemoveLastChar()
        {
            _cmd.Remove(_cmd.Length - 1, 1);
        }
        public void AddParameter(string key, Object value)
        {
            _pms.Add(key, value);
        }
        public DbCommand AsCommonCommand(DbConnection conn, DbTransaction trans)
        {
            StringBuilder cmd = this._cmd;
            for (int i = 0; i < _pms.Count; i++)
                if (_pms[i] is Func)
                {
                    cmd = cmd.Replace(_pms.GetName(i), (_pms[i] as Func).Content);
                    _pms.RemoveAt(i--);
                }
            DbCommand dbcmd = _dbprovider.Provider.CreateCommand();
            dbcmd.CommandText = cmd.ToString();
            dbcmd.Connection = conn;
            dbcmd.Transaction = trans;
            foreach (NameValue i in _pms)
            {
                DbParameter param = _dbprovider.Provider.CreateParameter();
                param.ParameterName = i.Name;
                param.Value = i.Value;
                (dbcmd.Parameters as IList).Add(param);
            }
            return dbcmd;
        }

        public int ExecuteNonQuery(Transaction trans)
        {
            DbConnection conn = null;
            bool err = false;
            try
            {
                DbCommand cmd;
                if (trans != null)
                    cmd = this.AsCommonCommand(trans.CurrentConnection.OriConnection, trans.CurrentTransaction);
                else
                {
                    conn = DbProvider.Current.CreateConnection();
                    conn.Open();
                    cmd = this.AsCommonCommand(conn, null);
                }
                return cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                err = true;
                if (trans != null)
                    trans.success = false;

                #if DEBUG
                    CommandException ce = new CommandException("ExecuteNonQuery", this, e);
                    Debug.WriteLine(ce);
                    throw ce;
                #endif

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
                    if (trans != null)
                        trans.success = false;

                    #if DEBUG
                        if (!err)
                        {
                            CommandException ce = new CommandException("ExecuteNonQuery", this, e);
                            Debug.WriteLine(ce);
                            throw ce;
                        }
                    #endif
                }
            }
        }
        public int ExecuteNonQuery()
        {
            return this.ExecuteNonQuery(null as Transaction);
        }
        public NameValueList[] Execute(Transaction trans)
        {
            DbConnection conn = null;
            DbDataReader r = null;
            bool err = false;
            try
            {
                DbCommand cmd;
                if (trans != null)
                    cmd = this.AsCommonCommand(trans.CurrentConnection.OriConnection, trans.CurrentTransaction);
                else
                {
                    conn = DbProvider.Current.CreateConnection();
                    conn.Open();
                    cmd = this.AsCommonCommand(conn, null);
                }
                r = cmd.ExecuteReader();
                List<NameValueList> res = new List<NameValueList>();
                List<string> keys = new List<string>();
                while (r.Read())
                {
                    NameValueList data = new NameValueList();
                    for (int i = 0; i < r.FieldCount; i++)
                    {
                        if (res.Count == 0)
                            keys.Add(r.GetName(i));
                        data.Add(keys[i], r.GetValue(i));
                    }
                    res.Add(data);
                }
                r.Close();
                return res.ToArray();
            }
            catch (Exception e)
            {
                err = true;
                if (trans != null)
                    trans.success = false;
                
                #if DEBUG
                    CommandException ce = new CommandException("Execute", this, e);
                    Debug.WriteLine(ce);
                    throw ce;
                #endif

                return new NameValueList[] { };
            }
            finally
            {
                try
                {
                    r.Close();
                }
                catch (Exception e)
                {
                    if (trans != null)
                        trans.success = false;

                    #if DEBUG
                        if (!err)
                        {
                            CommandException ce = new CommandException("Execute", this, e);
                            Debug.WriteLine(ce);
                            throw ce;
                        }
                    #endif
                }
                try
                {
                    if (trans == null)
                        conn.Close();
                }
                catch (Exception e)
                {
                    if (trans != null)
                        trans.success = false;

                    #if DEBUG
                        if (!err)
                        {
                            CommandException ce = new CommandException("Execute", this, e);
                            Debug.WriteLine(ce);
                            throw ce;
                        }
                    #endif
                }
            }
        }
        public NameValueList[] Execute()
        {
            return this.Execute(null as Transaction);
        }
        public Object ExecuteScalar(Transaction trans)
        {
            DbConnection conn = null;
            bool err = false;
            try
            {
                DbCommand cmd;
                if (trans != null)
                    cmd = this.AsCommonCommand(trans.CurrentConnection.OriConnection, trans.CurrentTransaction);
                else
                {
                    conn = DbProvider.Current.CreateConnection();
                    conn.Open();
                    cmd = this.AsCommonCommand(conn, null);
                }
                return cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                err = true;
                if (trans != null)
                    trans.success = false;

                #if DEBUG
                    CommandException ce = new CommandException("ExecuteScalar", this, e);
                    Debug.WriteLine(ce);
                    throw ce;
                #endif

                return null;
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
                    if (trans != null)
                        trans.success = false;

                    #if DEBUG
                        if (!err)
                        {
                            CommandException ce = new CommandException("ExecuteScalar", this, e);
                            Debug.WriteLine(ce);
                            throw ce;
                        }
                    #endif
                }
            }
        }
        public Object ExecuteScalar()
        {
            return this.ExecuteScalar(null as Transaction);
        }
        public bool ExecuteReader(DBReaderDelegate func, Transaction trans)
        {
            Debug.Assert(func != null, "DBReaderDelegate Cannot be NULL");
            DbConnection conn = null;
            DbDataReader r = null;
            bool err = false;
            try
            {
                DbCommand cmd;
                if (trans != null)
                    cmd = this.AsCommonCommand(trans.CurrentConnection.OriConnection, trans.CurrentTransaction);
                else
                {
                    conn = DbProvider.Current.CreateConnection();
                    conn.Open();
                    cmd = this.AsCommonCommand(conn, null);
                }
                r = cmd.ExecuteReader();
                while (r.Read() && func(r)) ;
                return true;
            }
            catch (Exception e)
            {
                err = true;
                if (trans != null)
                    trans.success = false;

                #if DEBUG
                    CommandException ce = new CommandException("ExecuteReader", this, e);
                    Debug.WriteLine(ce);
                    throw ce;
                #endif

                return false;
            }
            finally
            {
                try
                {
                    r.Close();
                }
                catch (Exception e)
                {
                    if (trans != null)
                        trans.success = false;

                    #if DEBUG
                        if (!err)
                        {
                            CommandException ce = new CommandException("ExecuteReader", this, e);
                            Debug.WriteLine(ce);
                            throw ce;
                        }
                    #endif
                }
                try
                {
                    if (trans == null)
                        conn.Close();
                }
                catch (Exception e)
                {
                    if (trans != null)
                        trans.success = false;

                    #if DEBUG
                        if (!err)
                        {
                            CommandException ce = new CommandException("ExecuteReader", this, e);
                            Debug.WriteLine(ce);
                            throw ce;
                        }
                    #endif
                }
            }
        }
        public bool ExecuteReader(DBReaderDelegate func)
        {
            return this.ExecuteReader(func, null as Transaction);
        }

        #region Static Methods

        public static int ExecuteNonQuery(params string[] cmds)
        {
            List<Command> commands = new List<Command>();
            foreach (string cmd in cmds)
                commands.Add(new Command(cmd));
            return commands.ExecuteNonQuery();
        }
        public static int ExecuteNonQuery(Transaction trans, params string[] cmds)
        {
            List<Command> commands = new List<Command>();
            foreach (string cmd in cmds)
                commands.Add(new Command(cmd));
            return commands.ExecuteNonQuery(trans);
        }
        public static NameValueList[] Execute(string cmd)
        {
            return new Command(cmd).Execute();
        }
        public static NameValueList[] Execute(Transaction trans, string cmd)
        {
            return new Command(cmd).Execute(trans);
        }
        public static Object ExecuteScalar(string cmd)
        {
            return new Command(cmd).ExecuteScalar();
        }
        public static Object ExecuteScalar(string cmd, Transaction trans)
        {
            return new Command(cmd).ExecuteScalar(trans);
        }
        public static bool ExecuteReader(string cmd, DBReaderDelegate func)
        {
            return new Command(cmd).ExecuteReader(func);
        }
        public static bool ExecuteReader(string cmd, DBReaderDelegate func, Transaction trans)
        {
            return new Command(cmd).ExecuteReader(func, trans);
        }
        
        #endregion
    }
}
