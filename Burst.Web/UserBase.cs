using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using Burst;
using Burst.Crypt;
using Burst.Data;
using Burst.Data.CommandBuilder;
using Burst.Data.Entity;

namespace Burst.Web
{
    [DataEntity("user")]
    public class UserBase<T> : DataEntity<T>
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

        protected override void SetValue(string Key, object Value)
        {
            if ((Key == "user_id"))
                this.__user_id = Convert.ToInt32(Value);
            if ((Key == "user_name"))
                this.__user_name = ((string)(Value));
            if ((Key == "password"))
                this.__password = ((string)(Value));
            if ((Key == "signup_time"))
                this.__signup_time = ((DateTime)(Value));
            base.SetValue(Key, Value);
        }

        public IPAddress IP;
        
        public static T Current
        {
            get
            {
                if (HttpContext.Current.Session == null)
                    return default(T);
                return (T)(HttpContext.Current.Session["Burst.Web.UserBase.Current"] as Object);
            }
        }

        #region 注册相关
        public static bool CheckExist(string username)
        {
            return UserBase<T>.CheckExist(new Where(string.Format("{0}=@0", DbProvider.Adapter.EnsureIdentifier("user_name")), username), null);
        }
        public static bool Signup(T user, HttpContext context)
        {
            (user as UserBase<T>).SignupTime = DateTime.Now;
            if ((user as DataEntity<T>).GetInsertCommand(InsertType.New).ExecuteNonQuery() > 0)
            {
                if (context != null)
                    Signin(user, null, context);
                return true;
            }
            return false;
        }
        #endregion

        #region 登录登出相关
        public static event UserStatusEventHandler SigningIn;
        public static event UserStatusEventHandler SignedIn;
        public static event UserStatusEventHandler SigningOut;
        public static event UserStatusEventHandler SignedOut;

        public static void Signin(T user, DateTime? expires, HttpContext context)
        {
            if (SigningIn != null)
            {
                UserStatusEventArgs e = new UserStatusEventArgs(context, false);
                SigningIn(user, e);
                if (e.Cancel)
                    return;
            }
            (user as UserBase<T>).IP = WebUtils.GetClientIP(context);
            HttpCookie hc = new HttpCookie("SInfo");
            hc.Values.Add(".b", CryptUtils.EncryptDES((user as UserBase<T>).UserName));
            hc.Values.Add(".a", CryptUtils.EncryptDES((user as UserBase<T>).Password));
            if (expires != null)
            {
                hc.Values.Add(".c", expires.ToString());
                hc.Expires = (DateTime)expires;
            }
            context.Response.Cookies.Set(hc);
            context.Session["Burst.Web.UserBase.Current"] = user;
            if (SignedIn != null)
                SignedIn(user, new UserStatusEventArgs(context, false));
        }
        public static void Signout(HttpContext context)
        {
            T user = default(T);
            if (SigningOut != null || SignedOut != null)
                user = (T)HttpContext.Current.Session["Burst.Web.UserBase.Current"];
            if(SigningOut != null)
            {
                UserStatusEventArgs e = new UserStatusEventArgs(context, false);
                SigningOut(user, e);
                if (e.Cancel)
                    return;
            }
            HttpContext.Current.Session.Remove("Burst.Web.UserBase.Current");
            HttpCookie hc = context.Request.Cookies["SInfo"];
            if (hc != null)
            {
                hc.Expires = DateTime.Now.AddDays(-1);
                context.Response.Cookies.Add(hc);
            }
            if (SignedOut != null)
                SignedOut(user, new UserStatusEventArgs(context, false));
        }
        public static bool TrySignin(string username, string password, DateTime? expires, HttpContext context)
        {
            T user = (Singleton as UserBase<T>).GetUser(username, password);
            if (user != null)
            {
                Signin(user, expires, context);
                return true;
            }
            else
                return false;
        }
        public static bool CheckSignin(HttpContext context)
        {
            T cu = Current;
            if (cu == null)
            {
                HttpCookie hc = context.Request.Cookies["SInfo"];
                if (hc == null)
                    return false;
                string username = hc.Values[".b"];
                string password = hc.Values[".a"];
                string s_expires = hc.Values[".c"];
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    return false;
                username = CryptUtils.DecryptDES(username);
                password = CryptUtils.DecryptDES(password);
                DateTime? expires = null;
                if (!string.IsNullOrEmpty(s_expires))
                {
                    try
                    {
                        expires = DateTime.Parse(s_expires);
                    }
                    catch { }
                }
                return TrySignin(username, password, expires, context);
            }
            else
                return true;
        }
        #endregion

        public virtual string Salt
        {
            get { return "burst0web0userbase"; }
        }

        public virtual string ObjectEncrypt(string username, string password)
        {
            return CryptUtils.Encrypt(CryptUtils.Encrypt(username + password, CryptType.MD5) + Salt, CryptType.MD5);
        }

        public virtual T GetUser(string username, string password)
        {
            return GetSingle(new Where(string.Format("{0}=@0 and {1}=@1",
                DbProvider.Adapter.EnsureIdentifier("user_name"),
                DbProvider.Adapter.EnsureIdentifier("password")
            ), username, password), null, null);
        }

        private static T _singleton;
        protected static T Singleton
        {
            get
            {

                if (_singleton == null)
                    _singleton = Activator.CreateInstance<T>();
                return _singleton;
            }
        }
        public static string Encrypt(string username, string password)
        {
            return (Singleton as UserBase<T>).ObjectEncrypt(username, password);
        }

        public static bool TryChangePassword(string oldPassword, string newPassword, HttpContext context)
        {
            T cu = Current;
            oldPassword = (cu as UserBase<T>).ObjectEncrypt((cu as UserBase<T>).UserName, oldPassword);
            newPassword = (cu as UserBase<T>).ObjectEncrypt((cu as UserBase<T>).UserName, newPassword);
            if ((Singleton as UserBase<T>).GetUser((cu as UserBase<T>).UserName, oldPassword) != null)
            {
                if (new Command(string.Format(
                        "update {0} set {1}=@0 where {2}=@1",
                        DbProvider.Adapter.EnsureIdentifier(TableName),
                        DbProvider.Adapter.EnsureIdentifier("password"),
                        DbProvider.Adapter.EnsureIdentifier("user_name")
                    ),
                    newPassword, (cu as UserBase<T>).UserName).ExecuteNonQuery() > -1)
                {
                    (cu as UserBase<T>).Password = newPassword;
                    DateTime? expires = null;
                    HttpCookie hc = context.Request.Cookies["SInfo"];
                    if (hc != null)
                    {
                        try
                        {
                            expires = DateTime.Parse(hc.Values[".c"]);
                        }
                        catch { }
                    }
                    Signin(cu, expires, context);
                    return true;
                }
                return false;
            }
            return false;
        }
    }
    public delegate void UserStatusEventHandler(Object sender, UserStatusEventArgs e);
    public class UserStatusEventArgs : EventArgs
    {
        public HttpContext Context;
        public bool Cancel;

        public UserStatusEventArgs(HttpContext Context, bool Cancel)
        {
            this.Context = Context;
            this.Cancel = Cancel;
        }
    }
}
