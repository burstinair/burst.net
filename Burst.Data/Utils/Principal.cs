using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.Security;
using System.Security.Principal;
using Burst;
using Burst.Crypt;
using Burst.Data;
using Burst.Data.Entity;

namespace Burst.Data.Utils
{
    [DataEntity("user")]
    public class Principal<T> : DataEntity<T>, IPrincipal
    {
        private int __user_id;
        [DataField("user_id")]
        public int UserId
        {
            get { return __user_id; }
            set { SetValue("user_id", value); }
        }

        private string __user_name;
        [DataField("user_name")]
        public string UserName
        {
            get { return __user_name; }
            set { SetValue("user_name", value); }
        }

        private string __password;
        [DataField("password")]
        public string Password
        {
            get { return __password; }
            set { SetValue("password", value); }
        }

        private DateTime __signup_time;
        [DataField("signup_time")]
        public DateTime SignupTime
        {
            get { return __signup_time; }
            set { SetValue("signup_time", value); }
        }

        protected internal override void SetValue(string Key, object Value)
        {
            if ((Key == "user_id"))
            {
                base.SetValue(Key, Value);
                this.__user_id = ((int)(Value));
            }
            if ((Key == "user_name"))
            {
                base.SetValue(Key, Value);
                this.__user_name = ((string)(Value));
            }
            if ((Key == "password"))
            {
                base.SetValue(Key, Value);
                this.__password = ((string)(Value));
            }
            if ((Key == "signup_time"))
            {
                base.SetValue(Key, Value);
                this.__signup_time = ((DateTime)(Value));
            }
        }

        public IPAddress IP;

        public IIdentity Identity
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }
}
