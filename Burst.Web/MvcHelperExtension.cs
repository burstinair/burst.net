using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

/*
using System.Web.Mvc;
namespace Burst.Web
{
    [Flags]
    public enum PagerMode
    {
        None = 0,
        Total = 1,
        PrevNext = 2,
        FirstLast = 4,
        AutoHide = 8,
        AsDropDown = 16
    }

    public static class MvcHelperExtension
    {
        /// <summary>
        /// 创建分页链接
        /// </summary>
        /// <param name="helper">HtmlHelper类</param>
        /// <param name="currentPage">当前页</param>
        /// <param name="totalPages">总页数</param>
        /// <param name="pagesToShow">前后显示的页数</param>
        /// <param name="mode">显示模式</param>
        public static MvcHtmlString Pager(this HtmlHelper helper, int currentPage, int totalPages, int pagesToShow, PagerMode mode)
        {
            RouteData routeData = helper.ViewContext.RouteData;

            string action = routeData.Values["action"].ToString().ToLower();
            string controller = routeData.Values["controller"].ToString().ToLower();
            StringBuilder html = new StringBuilder();

            string total = mode.HasFlag(PagerMode.Total) ? @"<span class=""total"">共" + totalPages + "页</span>" : "";

            string beforecontrol = "";
            if (mode.HasFlag(PagerMode.FirstLast) && !(mode.HasFlag(PagerMode.AutoHide) && currentPage == 1))
                beforecontrol = string.Format(@"<a class=""first"" href=""/{0}/{1}"">首页</a>", controller, action);
            if (mode.HasFlag(PagerMode.PrevNext) && !(mode.HasFlag(PagerMode.AutoHide) && currentPage == 1))
                beforecontrol = string.Format(@"{0}<a class=""prev"" href=""/{1}/{2}/{3}"">第一页</a>", beforecontrol, controller, action, currentPage - 1);

            string aftercontrol = "";
            if (mode.HasFlag(PagerMode.PrevNext) && !(mode.HasFlag(PagerMode.AutoHide) && currentPage == 1))
                aftercontrol = string.Format(@"<a class=""next"" href=""/{0}/{1}/{3}"">下一页</a>", controller, action, currentPage + 1);
            if (mode.HasFlag(PagerMode.FirstLast) && !(mode.HasFlag(PagerMode.AutoHide) && currentPage == 1))
                aftercontrol = string.Format(@"{0}<a class=""last"" href=""/{1}/{2}/{3}"">末页</a>", aftercontrol, controller, action, totalPages);
            
            html = Enumerable.Range(1, totalPages)
            .Where(i => (currentPage - pagesToShow) < i & i < (currentPage + pagesToShow))
            .Aggregate(new StringBuilder(@"<div class=""pagination"">" + total + beforecontrol), (seed, page) =>
            {
                if (page == currentPage)
                    seed.AppendFormat("<span>{0}</span>", page);
                else
                {
                    if (page == 1)
                        seed.AppendFormat("<a href=\"/{0}/{1}\">{2}</a>", controller, action, page);
                    else
                        seed.AppendFormat("<a href=\"/{0}/{1}/{2}\">{1}</a>", controller, action, page);
                }
                return seed;
            });
            html.Append(aftercontrol + @"</div>");
            return MvcHtmlString.Create(html.ToString());
        }
    }
}
*/