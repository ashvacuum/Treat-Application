using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static class ListExtensions 
    {
        public static T[] Shuffle<T>(this List<T> list)
        {
            var random = new System.Random();
            return list.OrderBy(x => random.Next()).ToArray();
        }
    }
}
