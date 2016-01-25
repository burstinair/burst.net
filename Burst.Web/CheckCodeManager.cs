using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Burst.Web
{
    public static class CheckCodeManager
    {
        internal static Cache<string, CheckCode> Cache = new Cache<string, CheckCode>();
        private static string _ccsbk = "CheckCodeBuffer";
        public static void UpdateSessionBuffer(string BufferKey, HttpContext context)
        {
            context.Session.Remove(_ccsbk);
            _ccsbk = BufferKey;
            context.Session[_ccsbk] = new LinkedList<string>();
        }

        private static LinkedList<string> GetCCB(HttpContext context)
        {
            LinkedList<string> res = context.Session[_ccsbk] as LinkedList<string>;
            if (res == null)
            {
                res = new LinkedList<string>();
                context.Session[_ccsbk] = res;
            }
            return res;
        }
        private static bool HasInSession(string key, HttpContext context)
        {
            LinkedList<string> ccb = GetCCB(context);
            return ccb.Contains(key);
        }
        private static void SetInSession(string key, HttpContext context)
        {
            LinkedList<string> ccb = GetCCB(context);
            ccb.AddLast(key);
            if (ccb.Count > 5)
                ccb.RemoveFirst();
        }
        public static bool Check(string cc, string ccid, HttpContext context)
        {
            if (string.IsNullOrEmpty(cc) || string.IsNullOrEmpty(ccid) || !HasInSession(ccid, context))
                return false;
            CheckCode tcc = Cache.GetCache(ccid) as CheckCode;
            if (tcc == null)
                return false;
            else if (tcc.code != cc.ToLower())
                return false;
            return true;
        }
        public static void Add(string ccid, CheckCode cc, HttpContext context)
        {
            Cache.SaveCache(ccid, cc);
            SetInSession(ccid, context);
        }
        public static void Remove(string ccid, HttpContext context)
        {
            Cache.RemoveCache(ccid);
            LinkedList<string> ccb = context.Session[_ccsbk] as LinkedList<string>;
            ccb.Remove(ccid);
        }
    }
}
