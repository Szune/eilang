using System;
using System.Linq;
using System.Reflection;

namespace eilang.Extensions
{
    public static class ReflectionExtensions
    {
        public static MemberInfo GetMemberInfo(this Type type, string name)
        {
            return type.GetMember(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).FirstOrDefault();
        }

        public static MemberInfo[] GetMemberInfos(this Type type)
        {
            return type
                .GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Concat((MemberInfo[])type.GetProperties(BindingFlags.Instance | BindingFlags.Public)).ToArray();
        }
        
        public static Type GetActualType(this MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo fi)
            {
                return fi.FieldType;
            }
            if (memberInfo is PropertyInfo pi)
            {
                return pi.PropertyType;
            }
            throw new InvalidOperationException($"Failed to get actual type, the {nameof(MemberInfo)} instance was neither a {nameof(FieldInfo)} nor a {nameof(PropertyInfo)}.");
        }

        public static void SetValue(this MemberInfo memberInfo, object instance, object value)
        {
            if (memberInfo is FieldInfo fi)
            {
                fi.SetValue(instance, value);
            }
            else if (memberInfo is PropertyInfo pi)
            {
                pi.SetValue(instance, value);
            }
            else
            {
                throw new InvalidOperationException($"Failed to set value, the {nameof(MemberInfo)} instance was neither a {nameof(FieldInfo)} nor a {nameof(PropertyInfo)}.");
            }
        }
        
        public static object GetValue(this MemberInfo memberInfo, object instance)
        {
            if (memberInfo is FieldInfo fi)
            {
                return fi.GetValue(instance);
            }
            else if (memberInfo is PropertyInfo pi)
            {
                return pi.GetValue(instance);
            }
            else
            {
                throw new InvalidOperationException($"Failed to set value, the {nameof(MemberInfo)} instance was neither a {nameof(FieldInfo)} nor a {nameof(PropertyInfo)}.");
            }
        }
    }
}