using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Burst.Data.CommandBuilder;
using Burst.Data.Entity;

namespace Burst.Data.Utils
{
    [DataEntity("log")]
    public class DbLog : DataEntity<DbLog>
    {
        private int __id;
        [DataField("id")]
        public int Id
        {
            get { return __id; }
            set { SetValue("id", value); }
        }

        private string __kind;
        [DataField("kind")]
        public string Kind
        {
            get { return __kind; }
            set { SetValue("kind", value); }
        }

        private DateTime __time;
        [DataField("time")]
        public DateTime Time
        {
            get { return __time; }
            set { SetValue("time", value); }
        }

        private string __content;
        [DataField("content")]
        public string Content
        {
            get { return __content; }
            set { SetValue("content", value); }
        }

        protected internal override void SetValue(string Key, object Value)
        {
            if ((Key == "id"))
            {
                base.SetValue(Key, Value);
                this.__id = ((int)(Value));
            }
            if ((Key == "kind"))
            {
                base.SetValue(Key, Value);
                this.__kind = ((string)(Value));
            }
            if ((Key == "time"))
            {
                base.SetValue(Key, Value);
                this.__time = ((DateTime)(Value));
            }
            if ((Key == "content"))
            {
                base.SetValue(Key, Value);
                this.__content = ((string)(Value));
            }
        }

        public static bool Append(string Kind, string Content)
        {
            try
            {
                DbLog log = new DbLog();
                log.Time = DateTime.Now;
                log.Kind = Kind;
                log.Content = Content;
                log.Insert(InsertType.New, null);
                return true;
            }
            catch { }
            return false;
        }
        public static bool Append(string Kind, string Content, Transaction trans)
        {
            try
            {
                DbLog log = new DbLog();
                log.Time = DateTime.Now;
                log.Kind = Kind;
                log.Content = Content;
                log.Insert(InsertType.New, trans);
                return true;
            }
            catch { }
            return false;
        }
        public static bool Append(Exception ex)
        {
            return Append("error", ex.Message + "\r\n\r\n" + ex.StackTrace);
        }
        public static bool Append(Exception ex, Transaction trans)
        {
            return Append("error", ex.Message + "\r\n\r\n" + ex.StackTrace, trans);
        }
    }
}
