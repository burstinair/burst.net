using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Data;

namespace Burst
{
    public static class IEnumerableExtension
    {
        public static int Count(this IEnumerable con)
        {
            int res = 0;
            foreach (var i in con)
                res++;
            return res;
        }

        public static void TrySwap(this IList list, int index1, int index2)
        {
            if (index2 < index1)
            {
                int _t = index1;
                index1 = index2;
                index2 = _t;
            }
            if (index1 < 0 || index2 >= list.Count)
                return;
            object t = list[index1];
            list[index1] = list[index2];
            list[index2] = t;
        }
    }

    public static class StringExtension
    {
        public static string FirstCharToUpper(this string ori)
        {
            if (ori.Length == 0)
                return ori;
            return char.ToUpper(ori[0]) + ori.Substring(1);
        }

        public static string[] SplitWithQuote(this string ori, string quote)
        {
            List<string> res = new List<string>();

            int i = ori.IndexOf(quote);
            while (i != -1)
            {
                int j = ori.IndexOf(quote, i + 1);
                if (j == -1)
                    throw new Exception("格式错误。");

                res.Add(ori.Substring(i + 1, j - i - 1));

                if (j == ori.Length)
                    break;

                i = ori.IndexOf(quote, j + 1);
            }

            return res.ToArray();
        }

        public static string GetHost(this string ori)
        {
            ori = ori.ToLower();
            if (Regex.IsMatch(ori, "^[a-z]+://.*"))
                ori = new Uri(ori.ToLower()).Host;
            string[] sections = ori.Split('.');
            if (sections.Length < 2)
                return null;
            return sections[sections.Length - 2] + '.' + sections[sections.Length - 1];
        }

        public static bool In(this string s, params string[] ps)
        {
            if (ps == null)
                return s == null;
            foreach (string t in ps)
                if (t.Equals(s, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            return false;
        }

        public static bool In(this string s, IEnumerable<string> ps)
        {
            if (ps == null)
                return s == null;
            foreach (string t in ps)
                if (t.Equals(s, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            return false;
        }
    }

    public static class TypeExtension
    {
    }

    public static class DataSetExtension
    {
        public static bool IsEmpty(this DataSet ds)
        {
            if (ds == null)
                return true;
            if (ds.Tables.Count <= 0)
                return true;
            if (ds.Tables[0].Rows.Count <= 0)
                return true;
            return false;
        }
    }
}
