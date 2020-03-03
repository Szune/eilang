using System;
using System.Linq;

namespace eilang.Extensions
{
    public static class EnumExtensions
    {
        public static string GetFlagsSeparatedByPipe<T>(this T e) where T : Enum
        {
            return string.Join(" | ",
                Enum.GetValues(typeof(T)).Cast<T>().Where(v => e.HasFlag(v)));
        }
    }
}