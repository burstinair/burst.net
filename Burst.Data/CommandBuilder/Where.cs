using System;
using System.Collections.Generic;
using System.Text;

namespace Burst.Data.CommandBuilder
{
    public enum WhereType
    {
        Key, Custom
    }
    public class Where
    {
        public object SingleObject { get; set; }
        public string QueryString { get; set; }
        public List<object> Parameters { get; set; }
        public WhereType Type { get; set; }
        public List<Order> Orders { get; set; }
        public Where()
        {
            this.QueryString = string.Empty;
            this.Parameters = new List<object>();
            this.Type = WhereType.Custom;
        }
        public Where(string where, params object[] pms)
        {
            this.QueryString = where;
            this.Parameters = new List<object>(pms);
            this.Type = WhereType.Custom;
        }
        public Where(string where, IEnumerable<Order> orders, params object[] pms)
        {
            this.QueryString = where;
            this.Orders = new List<Order>(orders);
            this.Parameters = new List<object>(pms);
            this.Type = WhereType.Custom;
        }
        public Where(WhereType type, object s)
        {
            this.Type = type;
            this.SingleObject = s;
        }
        public Where(WhereType type, object s, IEnumerable<Order> orders)
        {
            this.Type = type;
            this.SingleObject = s;
            this.Orders = new List<Order>(orders);
        }

        public void AddParameter(string value)
        {
            if (this.Parameters == null)
                this.Parameters = new List<object>();
            this.Parameters.Add(value);
        }
        public void AddOrder(string field, OrderType option)
        {
            if (this.Orders == null)
                this.Orders = new List<Order>();
            this.Orders.Add(new Order(field, option));
        }
    }
}
