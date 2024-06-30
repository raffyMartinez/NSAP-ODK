using System;

namespace NSAP_ODK.Mapping
{
    internal static class extension
    {
        public static void With<T>(this T obj, Action<T> a)
        {
            a(obj);
        }
    }
}