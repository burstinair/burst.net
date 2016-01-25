using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.Common;

namespace Burst.Data
{
    public class Transaction : IDisposable
    {
        public Connection Connection { get { return CurrentConnection; } }
        internal protected Connection CurrentConnection { get; set; }
        internal protected DbTransaction CurrentTransaction { get; set; }
        internal bool success;

        public Transaction()
        { }
        public Transaction(bool begin)
        {
            if (begin)
                Begin();
        }
        public Transaction(DbProvider provider)
        {
            Begin(provider);
        }

        public void Dispose()
        {
            try
            {
                CurrentTransaction.Rollback();
            }
            catch { }
            try
            {
                CurrentConnection._endTransaction(this);
            }
            catch { }
            CurrentTransaction = null;
            CurrentConnection = null;
        }

        public void Begin()
        {
            Begin(new Connection());
        }
        public void Begin(DbProvider provider)
        {
            Begin(new Connection(provider));
        }
        public void Begin(Connection connection)
        {
            try
            {
                CurrentConnection = connection;
                if (connection.Status == ConnectionStatus.Initialized)
                    CurrentConnection.Open();
                else if (connection.Status == ConnectionStatus.Closed)
                    throw new DbConnectException(new Exception("连接已关闭。"));
                connection._beginTransaction(this);
                CurrentTransaction = CurrentConnection.OriConnection.BeginTransaction();
                success = true;
            }
            catch (Exception e)
            {
                Dispose();

                #if DEBUG
                    Debug.WriteLine(e);
                    throw e;
                #endif
            }
        }
        public bool Commit()
        {
            try
            {
                if (success)
                    CurrentTransaction.Commit();
                return success;
            }
            catch (Exception e)
            {
                #if DEBUG
                    Debug.WriteLine(e);
                    throw e;
                #endif

                return false;
            }
            finally
            {
                Dispose();
            }
        }
        public void Rollback()
        {
            Dispose();
        }
    }
}
