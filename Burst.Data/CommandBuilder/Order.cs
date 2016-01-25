using System;
using System.Collections.Generic;
using System.Text;

namespace Burst.Data.CommandBuilder
{
    public enum OrderType
    {
        Asc, Desc
    }
    public class Order
    {
        public string Field { get; set; }
        public OrderType Type { get; set; }

        public Order(string field)
        {
            this.Field = field;
            this.Type = OrderType.Asc;
        }
        public Order(string field, OrderType type)
        {
            this.Field = field;
            this.Type = type;
        }

        protected virtual string sql_type
        {
            get
            {
                switch (Type)
                {
                    default:
                    case OrderType.Asc:
                        return "asc";
                    case OrderType.Desc:
                        return "desc";
                }
            }
        }
        public string ToString(IDbAdapter adapter)
        {
            return string.Format("{0} {1}", adapter.EnsureIdentifier(Field), sql_type);
        }
    }
}
