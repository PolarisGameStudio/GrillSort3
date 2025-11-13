using System;
using System.Collections.Generic;

namespace MyGame.Modules.Utils
{
    public static class ListHelper
    {
        public static T GetFirstGreaterThan<T>(this List<T> list, T value) where T : IComparable<T>
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].CompareTo(value) >= 0)
                {
                    return list[i];
                }
            }
            return default(T);
        }
    }
}