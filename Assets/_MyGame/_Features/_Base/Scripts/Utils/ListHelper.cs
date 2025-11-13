using System;
using System.Collections.Generic;
using System.Linq;

namespace MyGame.Modules.Utils
{
    public static class ListHelper
    {
        public static T GetFirstGreaterThan<T>(this List<T> list, T value) where T : IComparable<T>
        {
            list.OrderBy(e => -e.CompareTo(value)); // sort lại từ lớn đến nhỏ
            for (int i = 0; i < list.Count; i++)
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