using System;
using System.Collections.Generic;
using System.Linq;

namespace MyGame.Modules.Utils
{
    public static class ListHelper
    {
        public static T GetFirstGreaterThan<T>(this List<T> list, T value) where T : IComparable<T>
        {
            // list = list.OrderBy(e => -e.CompareTo(value)).ToList(); // sort lại từ lớn đến nhỏ
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (value.CompareTo(list[i]) >= 0)
                {
                    return list[i];
                }
            }
            return default(T);
        }
    }
}