using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Reflection;

namespace Burst
{
    public static class TypeUtils
    {
        public static bool IsSame<T, U>()
        {
            return typeof(T).Equals(typeof(U));
        }
        public static bool IsSame<T>(Type t)
        {
            return t.Equals(typeof(T));
        }
        public static bool IsSubclassOf<T, U>()
        {
            return typeof(T).IsSubclassOf(typeof(U));
        }
        public static bool IsSubclassOf<T>(Type t)
        {
            return t.IsSubclassOf(typeof(T));
        }
        public static bool IsAssignableFrom<T, U>()
        {
            return typeof(T).IsAssignableFrom(typeof(U));
        }
        public static bool IsAssignableFrom<T>(Type t)
        {
            return t.IsAssignableFrom(typeof(T));
        }
        public static bool Implements(this Type t, Type Interface)
        {
            if (t == null)
                return false;
            if (Interface == null)
                return false;
            if (!Interface.IsInterface)
                return false;
            foreach (Type _t in t.GetInterfaces())
                if (_t.Equals(Interface))
                    return true;
            return false;
        }
        public static bool IsPlainValue(this Type t)
        {
            if (t == typeof(string))
                return true;
            if (t == typeof(IPAddress))
                return true;
            if (t == typeof(IPEndPoint))
                return true;
            if (t == typeof(Encoding))
                return true;
            if (t == typeof(Uri))
                return true;
            return false;
        }

        public static Object GetDefaultValue(Type type)
        {
            if (type == typeof(Int16))
                return default(Int16);
            if (type == typeof(Int32))
                return default(Int32);
            if (type == typeof(Int64))
                return default(Int64);
            if (type == typeof(UInt16))
                return default(UInt16);
            if (type == typeof(UInt32))
                return default(UInt32);
            if (type == typeof(UInt64))
                return default(UInt64);
            if (type == typeof(Double))
                return default(Double);
            if (type == typeof(Single))
                return default(Single);
            if (type == typeof(Decimal))
                return default(Decimal);
            if (type == typeof(Boolean))
                return default(Boolean);
            if (type == typeof(DateTime))
                return default(DateTime);
            if (type == typeof(DateTimeOffset))
                return default(DateTimeOffset);
            if (type == typeof(TimeSpan))
                return default(TimeSpan);
            return null;
        }

        public static MemberInfo GetFieldOrProperty(this Type type, string name)
        {
            FieldInfo fi = type.GetField(name);
            if (fi != null)
                return fi;
            return type.GetProperty(name);
        }
        public static MemberInfo[] GetFieldsAndProperties(this Type type)
        {
            List<MemberInfo> res = new List<MemberInfo>(type.GetFields());
            foreach (PropertyInfo pi in type.GetProperties())
                if (pi.GetIndexParameters().Length == 0)
                    res.Add(pi);
            return res.ToArray();
        }
        public static Object GetValue(this MemberInfo mi, Object obj)
        {
            if (mi.MemberType == MemberTypes.Field)
                return (mi as FieldInfo).GetValue(obj);
            else if (mi.MemberType == MemberTypes.Property)
                if ((mi as PropertyInfo).CanRead)
                    return (mi as PropertyInfo).GetValue(obj, null);
            return null;
        }
        public static Type GetFieldOrPropertyType(this MemberInfo mi)
        {
            if (mi.MemberType == MemberTypes.Field)
                return (mi as FieldInfo).FieldType;
            else if (mi.MemberType == MemberTypes.Property)
                return (mi as PropertyInfo).PropertyType;
            return null;
        }
        public static void SetValue(this MemberInfo mi, Object obj, Object value)
        {
            if (mi.MemberType == MemberTypes.Field)
                (mi as FieldInfo).SetValue(obj, value);
            else if (mi.MemberType == MemberTypes.Property)
                if ((mi as PropertyInfo).CanWrite)
                    (mi as PropertyInfo).SetValue(obj, value, null);
        }

        public static Attribute GetAttribute(this MemberInfo mi, Type attributeType, bool inherit)
        {
            Attribute[] a = mi.GetCustomAttributes(attributeType, inherit) as Attribute[];
            if (a.Length > 0)
                return a[0];
            return null;
        }
        public static T GetAttribute<T>(this MemberInfo mi, bool inherit)
        {
            try
            {
                return (T)(GetAttribute(mi, typeof(T), inherit) as Object);
            }
            catch { }
            return default(T);
        }
        public static Attribute[] GetAttributes(this MemberInfo mi, Type attributeType, bool inherit)
        {
            return mi.GetCustomAttributes(attributeType, inherit) as Attribute[];
        }
        public static T[] GetAttributes<T>(this MemberInfo mi, bool inherit)
        {
            try
            {
                return GetAttributes(mi, typeof(T), inherit).Cast<T>().ToArray();
            }
            catch { }
            return new T[] { };
        }
        public static Attribute GetAttribute(this MemberInfo mi, Type attributeType)
        {
            return GetAttribute(mi, attributeType, false);
        }
        public static T GetAttribute<T>(this MemberInfo mi)
        {
            return GetAttribute<T>(mi, false);
        }
        public static Attribute[] GetAttributes(this MemberInfo mi, Type attributeType)
        {
            return GetAttributes(mi, attributeType, false);
        }
        public static T[] GetAttributes<T>(this MemberInfo mi)
        {
            return GetAttributes<T>(mi, false);
        }

        public static void CopyTo(this object ori, object target)
        {
            if (ori.GetType() != target.GetType())
                return;
            foreach (MemberInfo mi in ori.GetType().GetFieldsAndProperties())
                mi.SetValue(target, mi.GetValue(ori));
        }
    }
}
