using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.Collections.Generic
{
    public static class ListExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }

            return list;
        }
    }
}