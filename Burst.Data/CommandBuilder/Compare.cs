using System;
using System.Collections.Generic;
using System.Text;

namespace Burst.Data.CommandBuilder
{
    public enum CompareType
    {
        Custom, Equals, Unequals, GreaterThan, LessThan, Like, Between, In
    }
    [Flags]
    public enum CompareOption
    {
        None = 0, IgnoreEmptyString = 1, IgnoreEmptyDateTime = 2, ConvertDateTime = 4
    }
    public class Compare : ICloneable
    {
        public string Field { get; set; }
        public string CustomCondition { get; set; }
        public object[] CompareValues { get; set; }
        public CompareType Type { get; set; }
        public CompareOption Option { get; set; }

        public Compare()
        {
        }
        public Compare(string customCondition)
        {
            this.CustomCondition = customCondition;
            this.Type = CompareType.Custom;
            this.Option = CompareOption.IgnoreEmptyString | CompareOption.ConvertDateTime;
        }
        public Compare(string fieldName, params object[] compareValues)
        {
            this.Field = fieldName;
            this.CompareValues = compareValues;
            if (compareValues.Length > 2)
                Type = CompareType.In;
            else if (compareValues.Length == 2)
                Type = CompareType.Between;
            else
                Type = CompareType.Equals;
            this.Option = CompareOption.IgnoreEmptyString | CompareOption.ConvertDateTime;
        }
        public Compare(CompareType type, string fieldName, params object[] compareValues)
        {
            this.Type = type;
            this.Field = fieldName;
            this.CompareValues = compareValues;
            this.Option = CompareOption.IgnoreEmptyString | CompareOption.ConvertDateTime;
        }
        public Compare(CompareType type, CompareOption option, string fieldName, params object[] compareValues)
        {
            this.Type = type;
            this.Field = fieldName;
            this.CompareValues = compareValues;
            this.Option = option;
        }

        public object Clone()
        {
            Compare copy = new Compare();
            copy.Field = this.Field;
            copy.CustomCondition = this.CustomCondition;
            copy.CompareValues = this.CompareValues.Clone() as object[];
            copy.Type = this.Type;
            copy.Option = this.Option;
            return copy;
        }

        public bool IsSameConditionTo(Compare compare)
        {
            if (compare == null)
                return false;
            if (this.Type != compare.Type)
                return false;
            if (this.Type == CompareType.Custom)
                return this.CustomCondition == compare.CustomCondition;
            return this.Field == compare.Field && this.Type == compare.Type;
        }
    }

    public static class CompareExtension
    {
        private static bool _compareGetSingleValue(Compare co)
        {
            if (co.CompareValues.Length == 0)
                return false;
            if ((co.CompareValues[0] is string || co.CompareValues[0] == null) &&
                string.IsNullOrEmpty(co.CompareValues[0] as string) &&
                co.Option.HasFlag(CompareOption.IgnoreEmptyString)
            )
                return false;
            return true;
        }
        private static bool _compareGetBetweenValue(Compare co)
        {
            if (co.CompareValues.Length < 2)
                return false;
            if (co.CompareValues[0] is DateTime && co.CompareValues[1] is DateTime)
            {
                if (co.Option.HasFlag(CompareOption.IgnoreEmptyDateTime))
                {
                    if ((DateTime)co.CompareValues[0] == default(DateTime))
                        return false;
                    if ((DateTime)co.CompareValues[1] == default(DateTime))
                        return false;
                }
                else if (co.Option.HasFlag(CompareOption.ConvertDateTime))
                {
                    if ((DateTime)co.CompareValues[0] == default(DateTime))
                        if ((DateTime)co.CompareValues[1] == default(DateTime))
                            return false;
                        else
                            co.CompareValues[0] = DateTime.MinValue;
                    else if ((DateTime)co.CompareValues[1] == default(DateTime))
                        co.CompareValues[1] = DateTime.MaxValue;
                }
            }
            return true;
        }
        public static bool _compareGetInValue(Compare co)
        {
            if (co.CompareValues.Length == 0)
                return false;
            return true;
        }

        public static Where ToWhere(this Compare _co, IDbAdapter adapter)
        {
            Compare co = _co.Clone() as Compare;
            switch (co.Type)
            {
                default:
                case CompareType.Custom:
                    return new Where(co.CustomCondition, co.CompareValues);
                case CompareType.Equals:
                    if (_compareGetSingleValue(co))
                        return new Where(string.Format("{0}=@0", adapter.EnsureIdentifier(co.Field)), co.CompareValues[0]);
                    return null;
                case CompareType.Unequals:
                    if (_compareGetSingleValue(co))
                        return new Where(string.Format("{0}<>@0", adapter.EnsureIdentifier(co.Field)), co.CompareValues[0]);
                    return null;
                case CompareType.GreaterThan:
                    if (_compareGetSingleValue(co))
                        return new Where(string.Format("{0}>@0", adapter.EnsureIdentifier(co.Field)), co.CompareValues[0]);
                    return null;
                case CompareType.LessThan:
                    if (_compareGetSingleValue(co))
                        return new Where(string.Format("{0}<@0", adapter.EnsureIdentifier(co.Field)), co.CompareValues[0]);
                    return null;
                case CompareType.Like:
                    if (_compareGetSingleValue(co))
                        return new Where(string.Format("{0} like {1}",
                            adapter.EnsureIdentifier(co.Field), adapter.Concat("'%'", "@0", "'%'")), adapter.LikePattern(co.CompareValues[0].ToString()));
                    return null;
                case CompareType.Between:
                    if (_compareGetBetweenValue(co))
                        return new Where(string.Format("{0} between @0 and @1", adapter.EnsureIdentifier(co.Field)), co.CompareValues[0], co.CompareValues[1]);
                    return null;
                case CompareType.In:
                    if (_compareGetInValue(co))
                    {
                        List<object> in_pms = new List<object>();
                        StringBuilder in_query = new StringBuilder();
                        foreach (var o in co.CompareValues)
                        {
                            in_query.AppendFormat("@{0},", in_pms.Count);
                            in_pms.Add(o);
                        }
                        in_query.Remove(in_query.Length - 1, 1);
                        return new Where(string.Format("{0} in ({1})", adapter.EnsureIdentifier(co.Field), in_query), in_pms.ToArray());
                    }
                    return null;
            }
        }
        public static Where ToWhere(this Compare _co)
        {
            return ToWhere(_co, DbProvider.Current.Adapter);
        }
        public static Where ToWhere(this IEnumerable<Compare> cos, IDbAdapter adapter)
        {
            StringBuilder where = new StringBuilder();
            List<Object> pms = new List<Object>();
            foreach (Compare _co in cos)
            {
                Compare co = _co.Clone() as Compare;
                switch (co.Type)
                {
                    default:
                    case CompareType.Custom:
                        where.AppendFormat("{0} and ", co.CustomCondition);
                        pms.AddRange(co.CompareValues);
                        break;
                    case CompareType.Equals:
                        if (_compareGetSingleValue(co))
                        {
                            where.AppendFormat("{0}=@{1} and ", adapter.EnsureIdentifier(co.Field), pms.Count);
                            pms.Add(co.CompareValues[0]);
                        }
                        break;
                    case CompareType.Unequals:
                        if (_compareGetSingleValue(co))
                        {
                            where.AppendFormat("{0}<>@{1} and ", adapter.EnsureIdentifier(co.Field), pms.Count);
                            pms.Add(co.CompareValues[0]);
                        }
                        break;
                    case CompareType.GreaterThan:
                        if (_compareGetSingleValue(co))
                        {
                            where.AppendFormat("{0}>@{1} and ", adapter.EnsureIdentifier(co.Field), pms.Count);
                            pms.Add(co.CompareValues[0]);
                        }
                        break;
                    case CompareType.LessThan:
                        if (_compareGetSingleValue(co))
                        {
                            where.AppendFormat("{0}<@{1} and ", adapter.EnsureIdentifier(co.Field), pms.Count);
                            pms.Add(co.CompareValues[0]);
                        }
                        break;
                    case CompareType.Like:
                        if (_compareGetSingleValue(co))
                        {
                            where.AppendFormat("{0} like {1} and ", adapter.EnsureIdentifier(co.Field), adapter.Concat("'%'", "@" + pms.Count, "'%'"));
                            pms.Add(adapter.LikePattern(co.CompareValues[0].ToString()));
                        }
                        break;
                    case CompareType.Between:
                        if (_compareGetBetweenValue(co))
                        {
                            where.AppendFormat("{0} between @{1} and @{2} and ", adapter.EnsureIdentifier(co.Field), pms.Count, pms.Count + 1);
                            pms.Add(co.CompareValues[0]);
                            pms.Add(co.CompareValues[1]);
                        }
                        break;
                    case CompareType.In:
                        if (_compareGetInValue(co))
                        {
                            where.AppendFormat("{0} in (", adapter.EnsureIdentifier(co.Field));
                            foreach (var o in co.CompareValues)
                            {
                                where.AppendFormat("@{0},", pms.Count);
                                pms.Add(o);
                            }
                            where.Remove(where.Length - 1, 1);
                            where.Append(") and ");
                        }
                        break;
                }
            }
            if (where.Length == 0)
                return null;
            where.Remove(where.Length - 5, 5);
            return new Where(where.ToString(), pms.ToArray());
        }
        public static Where ToWhere(this IEnumerable<Compare> cos)
        {
            return ToWhere(cos, DbProvider.Current.Adapter);
        }
        public static Compare FindSameCondition(this IEnumerable<Compare> cos, Compare co)
        {
            foreach (Compare _co in cos)
                if (_co.IsSameConditionTo(co))
                    return _co;
            return null;
        }
    }
}
