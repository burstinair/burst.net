using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Burst.Distribution
{
    /// <summary>
    /// 用于管理分布式分组
    /// </summary>
    /// <remarks>
    /// 2010/09/19 Burst-最后一次修改
    /// </remarks>
    public class HostGroup : NameObjectCollectionBase
    {
        protected static HostGroup Singleton = new HostGroup();
        /// <summary>
        /// 获取指定主机所在域中的所有WebService
        /// </summary>
        /// <param name="rh">指定的主机</param>
        /// <returns>得到的WebService</returns>
        public static string[] getServices(ResHost rh)
        {
            return getServices(rh.groups.ToArray());
        }
        /// <summary>
        /// 获取所有指定分组中的所有WebService
        /// </summary>
        /// <param name="groups">指定的分组</param>
        /// <returns>得到的WebService</returns>
        public static string[] getServices(string[] groups)
        {
            if (groups == null)
                return new string[0];
            List<string> res = new List<string>();
            for (int i = 0; i < groups.Length; i++)
            {
                List<ResHost> list = Singleton.BaseGet(groups[i]) as List<ResHost>;
                if (list != null)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (res.Contains(list[j].searchService))
                            continue;
                        res.Add(list[j].searchService);
                    }
                }
            }
            return res.ToArray();
        }

        /// <summary>
        /// 向指定分组中添加主机
        /// </summary>
        /// <param name="rh">要添加的主机</param>
        /// <param name="name">要添加到的分组的名字</param>
        /// <returns>是否成功</returns>
        public static bool add(ResHost rh, string name)
        {
            try
            {
                if (Singleton.BaseGet(name) == null)
                    Singleton.BaseSet(name, new List<ResHost>());
                List<ResHost> list = Singleton.BaseGet(name) as List<ResHost>;
                list.Add(rh);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 从指定分组中删除主机
        /// </summary>
        /// <param name="rh">要删除的主机</param>
        /// <param name="name">要从中删除的分组的名字</param>
        /// <returns></returns>
        public static bool remove(ResHost rh, string name)
        {
            try
            {
                if (Singleton.BaseGet(name) == null)
                    Singleton.BaseSet(name, new List<ResHost>());
                List<ResHost> list = Singleton.BaseGet(name) as List<ResHost>;
                list.Remove(rh);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 从所有分组中删除主机
        /// </summary>
        /// <param name="rh">要删除的主机</param>
        public static void remove(ResHost rh)
        {
            for (int i = 0; i < Singleton.Count; i++)
            {
                if (Singleton.BaseGet(i) as List<ResHost> != null)
                {
                    List<ResHost> list = Singleton.BaseGet(i) as List<ResHost>;
                    list.Remove(rh);
                }
            }
        }
        /// <summary>
        /// 判断指定分组是否包含指定主机
        /// </summary>
        /// <param name="rh">指定的主机</param>
        /// <param name="name">指定的分组的名字</param>
        /// <returns>指定分组是否包含指定主机</returns>
        public static bool contains(ResHost rh, string name)
        {
            try
            {
                if (Singleton.BaseGet(name) == null)
                    Singleton.BaseSet(name, new List<ResHost>());
                List<ResHost> list = Singleton.BaseGet(name) as List<ResHost>;
                return list.Contains(rh);
            }
            catch { return false; }
        }

        /// <summary>
        /// 获取一个新的分组
        /// </summary>
        /// <returns>获取到的新的分组的名字</returns>
        public static string getNewGroup()
        {
            int p = 1;
            while (Singleton.BaseGet("g" + p) != null)
                p++;
            return "g" + p;
        }
        /// <summary>
        /// 获取指定分组中的所有主机
        /// </summary>
        /// <param name="name">分组名</param>
        /// <returns>获取到的主机</returns>
        public static ResHost[] getResHosts(string name)
        {
            if (Singleton.BaseGet(name) as List<ResHost> == null)
                return null;
            else
                return (Singleton.BaseGet(name) as List<ResHost>).ToArray();
        }
        /// <summary>
        /// 清空分组
        /// </summary>
        public static void clear()
        {
            Singleton.BaseClear();
        }
    }
}
