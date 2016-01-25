using System;
using System.Collections.Generic;
using System.Text;

namespace Burst.Web
{
    public class SuccessException : BaseException
    {
        public override string Status
        {
            get { return "success"; }
        }

        private static SuccessException _singleton;
        public static SuccessException Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new SuccessException();
                return _singleton;
            }
        }

        public SuccessException() : base("操作成功。", null) { }
        public SuccessException(string message) : base(message, null) { }
    }
}
