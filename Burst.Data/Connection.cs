using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Diagnostics;

namespace Burst.Data
{
    public enum ConnectionStatus
    {
        Initialized, Opend, Closed
    }

    public class Connection
    {
        protected List<Transaction> _transactions;
        public List<Transaction> Transactions
        {
            get
            {
                return _transactions;
            }
        }

        protected DbConnection _oriConnection;
        internal DbConnection OriConnection
        {
            get
            {
                return _oriConnection;
            }
        }
        public ConnectionStatus Status { get; set; }

        public Connection()
        {
            _oriConnection = DbProvider.Current.CreateConnection();
            _transactions = new List<Transaction>();
            this.Status = ConnectionStatus.Initialized;
        }
        public Connection(DbProvider provider)
        {
            _oriConnection = provider.CreateConnection();
            _transactions = new List<Transaction>();
            this.Status = ConnectionStatus.Initialized;
        }

        public void Open()
        {
            Debug.Assert(Status == ConnectionStatus.Initialized, "Connection.Open: Status must be Initialized.");
            if (Status == ConnectionStatus.Initialized)
            {
                try
                {
                    _oriConnection.Open();
                    Status = ConnectionStatus.Opend;
                }
                catch (Exception e)
                {
                    #if DEBUG
                        DbConnectException dce = new DbConnectException(e);
                        Debug.WriteLine(dce);
                        throw dce;
                    #endif
                }
            }
        }
        public void Close()
        {
            Debug.Assert(Status == ConnectionStatus.Opend, "Connection.Close: Status must be Opened.");
            if (Status == ConnectionStatus.Opend)
            {
                try
                {
                    while (_transactions.Count > 0)
                        _transactions[0].Dispose();
                    Status = ConnectionStatus.Closed;
                }
                catch { }
            }
        }
        public Transaction BeginTransaction()
        {
            Debug.Assert(Status == ConnectionStatus.Opend, "Connection.BeginTransaction: Status must be Opened.");
            if (Status == ConnectionStatus.Opend)
            {
                Transaction res = new Transaction();
                res.Begin(this);
                return res;
            }
            return null;
        }
        internal void _beginTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
        }
        internal void _endTransaction(Transaction transaction)
        {
            _transactions.Remove(transaction);
            if (_transactions.Count == 0)
                try
                {
                    _oriConnection.Close();
                    Status = ConnectionStatus.Closed;
                }
                catch { }
        }
    }
}
