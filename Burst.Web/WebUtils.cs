using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using System.Net;
using System.Xml.Linq;
using System.Web.UI.HtmlControls;
using Burst.Data.Utils;

namespace Burst.Web
{
    public class WebUtils
    {
        public static string EncodeHtmlAndCRLF(string source)
        {
            if (source == null)
                return null;
            return HttpUtility.HtmlEncode(Utils.EncodeCRLF(source));
        }
        public static string DecodeHtmlAndCRLF(string source)
        {
            if (source == null)
                return null;
            return Utils.DecodeCRLF(HttpUtility.HtmlDecode(source));
        }

        public static string UrlEncode(string source)
        {
            source = HttpUtility.UrlEncode(source);
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            return source.Replace("+", "%20");
        }

        public static IPAddress GetClientIP(HttpContext context)
        {
            IPAddress res;
            string ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            try
            {
                if (string.IsNullOrEmpty(ip))
                    res = IPAddress.Parse(context.Request.UserHostAddress);
                else
                    res = IPAddress.Parse(ip);
            }
            catch
            {
                res = IPAddress.Parse(context.Request.UserHostAddress);
            }
            return res;
        }

        public static string Path(string comp)
        {
            string cp = HttpRuntime.AppDomainAppVirtualPath;
            if (cp[cp.Length - 1] != '/')
                cp += '/';
            if(string.IsNullOrEmpty(comp))
                return cp;
            if (comp[0] == '/' || comp[0] == '\\')
                return cp + comp.Substring(1);
            else
                return cp + comp;
        }

        public static string PhysicalPath(string comp)
        {
            string cp = HttpRuntime.AppDomainAppPath;
            if (cp[cp.Length - 1] != '\\')
                cp += '\\';
            if(string.IsNullOrEmpty(comp))
                return cp;
            if (comp[0] == '/' || comp[0] == '\\')
                return cp + comp.Substring(1);
            else
                return cp + comp;
        }

        public static string UTF8_C2A0 = Encoding.UTF8.GetString(new byte[] { 0xc2, 0xa0 });

        public static string DealException(Exception ex, bool writeLog)
        {
            if (ex == null)
                return "unknown";
            var msg = "unknown";
            if (ex is HttpException)
            {
                int code = (ex as HttpException).GetHttpCode();
                if (code != 500)
                    writeLog = false;
                if (ex.InnerException != null)
                    ex = ex.InnerException;
                msg = code.ToString();
            }
            if (ex is HttpRequestValidationException)
            {
                ex = new XSSException(GetClientIP(HttpContext.Current), ex);
                msg = "xss";
            }
            if (writeLog)
                DbLog.Append(ex);
            return msg;
        }

        private static void _grep_user_input_html(XElement m)
        {
            if (m.Name.LocalName.ToLower().In(
                "html", "body", "meta", "frame", "frameset", "iframe",
                "layer", "ilayer", "applet", "script", "style", "embed",
                "object", "link", "form", "input", "textarea", "select"
            ))
                m.Remove();
            if (m.Name.LocalName.ToLower() == "a")
            {
                foreach (XAttribute href in m.Attributes())
                    if (href.Name.LocalName.ToLower() == "href")
                    {
                        if (href.Value.TrimStart().IndexOf("javascript:", StringComparison.CurrentCultureIgnoreCase) == 0)
                            href.Remove();
                    }
            }
            else if (m.Name.LocalName.ToLower() == "img")
            {
                foreach (XAttribute src in m.Attributes())
                    if (src.Name.LocalName.ToLower() == "src")
                    {
                        if (src.Value.TrimStart().IndexOf("javascript:", StringComparison.CurrentCultureIgnoreCase) == 0)
                            src.Remove();
                    }
            }
            foreach (XAttribute attr in m.Attributes())
                if (attr.Name.LocalName.ToLower() == "style")
                {
                    if (attr.Value.IndexOf("expression", StringComparison.CurrentCultureIgnoreCase) != -1)
                        attr.Remove();
                }
                else if (attr.Name.LocalName.ToLower().In("onload", "onunload", "onchange",
                    "onsubmit", "onreset", "onselect", "onblur", "onfocus", "onabort",
                    "onkeydown", "onkeyup", "onkeypress", "onclick", "ondblclick",
                    "onmousedown", "onmouseup", "onmousemove", "onmouseout", "onmouseover"
                ))
                    attr.Remove();
            foreach (XElement xe in m.Elements())
                _grep_user_input_html(xe);
        }
        public static string GrepUserInputHtml(string Source)
        {
            Source = Regex.Replace(Source, "<[a-zA-Z0-9_]+:.+?>", "");
            Source = Regex.Replace(Source, "</[a-zA-Z0-9_]+:[a-zA-Z0-9_]+>", "");
            Source = Source.Replace("&", "&amp;");
            XElement m;
            try
            {
                m = XElement.Parse(Source);
            }
            catch
            {
                m = XElement.Parse(string.Format("<div>{0}</div>", Source));
            }
            _grep_user_input_html(m);
            return m.ToString().Replace("&amp;", "&");
        }

        private static string _get_first_image_url(XElement m)
        {
            if (m.Name.LocalName.ToLower() == "img")
                foreach (XAttribute src in m.Attributes())
                    if (src.Name.LocalName.ToLower() == "src")
                        return src.Value;
            foreach (XElement xe in m.Elements())
            {
                string res = _get_first_image_url(xe);
                if (res != null)
                    return res;
            }
            return null;
        }
        public static string GetFirstImageUrl(string Source)
        {
            Source = Source.Replace("&", "&amp;");
            XElement m;
            try
            {
                m = XElement.Parse(Source);
            }
            catch
            {
                m = XElement.Parse(string.Format("<div>{0}</div>", Source));
            }
            return _get_first_image_url(m);
        }

        public static string GetXmlText(string input)
        {
            return Regex.Replace(input, "<[^>]*>", "").Replace("&nbsp;", " ");
        }

        public static string TextToHtml(string source)
        {
            if (source == null)
                return null;
            source = EncodeHtmlAndCRLF(source);
            string res = "";
            int pre = 0;
            int p = source.IndexOf('\\');
            while (p != -1)
            {
                switch (source[p + 1])
                {
                    case 'n':
                        res = string.Format("{0}<p>{1}</p>",
                            res, source.Substring(pre, p - pre));
                        break;
                    case '\\':
                        res = string.Format("{0}{1}\\",
                            res, source.Substring(pre, p - pre));
                        break;
                }
                pre = p + 2;
                p = source.IndexOf('\\', p + 2);
            }
            if (pre < source.Length)
                res = string.Format("{0}<p>{1}</p>",
                    res, source.Substring(pre, source.Length - pre));
            return res;
        }

        public static HtmlGenericControl JSTag(string script)
        {
            HtmlGenericControl hgcLoadJs = new HtmlGenericControl();
            hgcLoadJs.TagName = "script";
            hgcLoadJs.Attributes.Add("type", "text/javascript");
            hgcLoadJs.InnerHtml = script;
            return hgcLoadJs;
        }

        public static HtmlGenericControl Tag(string tagname, string content, params NameValue[] attributes)
        {
            HtmlGenericControl res = new HtmlGenericControl(tagname);
            res.InnerHtml = content;
            foreach (var i in attributes)
                res.Attributes.Add(i.Name, i.Value.ToString());
            return res;
        }
        public static HtmlGenericControl Tag(string tagname, IEnumerable<HtmlGenericControl> content, params NameValue[] attributes)
        {
            HtmlGenericControl res = new HtmlGenericControl(tagname);
            foreach (var i in content)
                res.Controls.Add(i);
            foreach (var i in attributes)
                res.Attributes.Add(i.Name, i.Value.ToString());
            return res;
        }

        /// <summary>
        /// Web响应异步回调函数
        /// </summary>
        /// <param name="r"></param>
        private static void ResponseCallBack(IAsyncResult r)
        {
            Stream s = null;
            AsyncState state = r.AsyncState as AsyncState;
            try
            {
                s = (state.r.EndGetResponse(r) as HttpWebResponse).GetResponseStream();
            }
            catch { }
            if (state.dele != null)
                state.dele(s);
            if (s == null)
                return;
            s.Close();
        }
        /// <summary>
        /// 开始请求响应
        /// </summary>
        /// <param name="state"></param>
        private static void AsyncResponse(AsyncState state)
        {
            try
            {
                state.r.BeginGetResponse(new AsyncCallback(ResponseCallBack), state);
            }
            catch
            {
                if (state.dele != null)
                    state.dele(null);
            }
        }
        /// <summary>
        /// Web请求异步回调函数
        /// </summary>
        /// <param name="r"></param>
        private static void RequestCallBack(IAsyncResult r)
        {
            AsyncState state = r.AsyncState as AsyncState;
            try
            {
                Stream s = state.r.EndGetRequestStream(r);
                s.Write(state.data, 0, state.data.Length);
                s.Close();
                AsyncResponse(state);
            }
            catch
            {
                if (state.dele != null)
                    state.dele(null);
            }
        }
        /// <summary>
        /// 开始请求
        /// </summary>
        /// <param name="state"></param>
        private static void AsyncRequest(AsyncState state)
        {
            try
            {
                state.r.BeginGetRequestStream(new AsyncCallback(RequestCallBack), state);
            }
            catch
            {
                if (state.dele != null)
                    state.dele(null);
            }
        }
        /// <summary>
        /// 向指定url发送一段数据，并等待异步返回
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <param name="dele"></param>
        public static HttpWebResponse Request(string url, NameValueCollection data, RequestMethod method, Action<Stream> dele, CookieContainer cc, string referer)
        {
            StringBuilder query = new StringBuilder();
            byte[] postdata = null;
            if (data != null)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    query.Append(HttpUtility.UrlEncode(data.GetKey(i), Encoding.UTF8));
                    query.Append("=");
                    query.Append(HttpUtility.UrlEncode(data[i], Encoding.UTF8));
                    if (i != data.Count - 1)
                        query.Append("&");
                }
                switch (method)
                {
                    case RequestMethod.Get:
                        Uri uri = new Uri(url);
                        url = string.Format("{0}?{1}", uri.AbsolutePath, query);
                        if (!string.IsNullOrEmpty(uri.Query))
                            url = string.Format("{0}&{1}", url, uri.Query);
                        break;
                    case RequestMethod.Post:
                        postdata = Encoding.ASCII.GetBytes(query.ToString());
                        break;
                }
            }
            AsyncState state = new AsyncState(postdata, dele, url, method, cc);
            state.r.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.10 (KHTML, like Gecko) Chrome/23.0.1262.0 Safari/537.10";
            state.r.Referer = referer;
            state.r.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            state.r.AllowAutoRedirect = true;
            state.r.KeepAlive = true;
            if (dele == null)
            {
                if (state.data != null)
                {
                    state.r.ContentLength = state.data.Length;
                    Stream s = state.r.GetRequestStream();
                    s.Write(state.data, 0, state.data.Length);
                    s.Close();
                }
                return state.r.GetResponse() as HttpWebResponse;
            }
            else
            {
                if (state.data != null)
                    AsyncRequest(state);
                else
                    AsyncResponse(state);
            }
            return null;
        }
        /// <summary>
        /// WebService调用异步回调函数
        /// </summary>
        /// <param name="r"></param>
        private static void EndFunctionService(IAsyncResult r)
        {
            AsyncState state = r.AsyncState as AsyncState;
            string res = state.serviceInvoker.EndFunction(r);
            if (res == null)
                state.serviceInvoker.BeginFunction(state.key, 0, new AsyncCallback(EndFunctionService), state);
            else if (state.WSdele != null)
                state.WSdele(res);
        }
        /// <summary>
        /// 开始使用WebService
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public static void FunctionService(string key, Action<string> callback)
        {
            //功能
            AsyncState state = new AsyncState(key, callback, "", null);
            state.serviceInvoker.BeginFunction(key, 0, new AsyncCallback(EndFunctionService), state);
        }
    }
    /// <summary>
    /// 异步状态对象
    /// </summary>
    internal class AsyncState
    {
        //WebRequest的State
        internal HttpWebRequest r;
        internal string url;
        internal RequestMethod method;
        internal byte[] data;
        internal Action<Stream> dele;
        internal CookieContainer cc;

        internal AsyncState(byte[] data, Action<Stream> dele, string url, RequestMethod method, CookieContainer cc)
        {
            this.data = data;
            this.dele = dele;
            this.url = url;
            this.method = method;
            this.cc = cc;
            this.r = HttpWebRequest.Create(url) as HttpWebRequest;
            this.r.Method = method.ToString().ToUpper();
            this.r.CookieContainer = cc;
            this.r.ContentType = "application/x-www-form-urlencoded";
        }

        //WebService的State
        internal IServiceInvoker serviceInvoker;
        internal Type serviceInvokerType;
        internal string key;
        internal Action<string> WSdele;

        internal AsyncState(string key, Action<string> dele, string url, Type serviceInvokerType)
        {
            this.key = key;
            this.url = url;
            this.WSdele = dele;
            this.serviceInvokerType = serviceInvokerType;
            this.serviceInvoker = Activator.CreateInstance(serviceInvokerType) as IServiceInvoker;
            this.serviceInvoker.Initialize(url);
        }
    }
    public enum RequestMethod
    {
        /// <summary>
        /// 表示采用Post方法
        /// </summary>
        Post,

        /// <summary>
        /// 表示采用Get方法
        /// </summary>
        Get
    }
}
