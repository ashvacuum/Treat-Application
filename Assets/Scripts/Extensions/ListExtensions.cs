using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static class ListExtensions 
    {
        public static void Shuffle<T>(this List<T> list)
        {
            var random = new Random();
            var n = list.Count;
        
            // Start from the last element and swap it with a randomly selected element
            for (var i = n - 1; i > 0; i--)
            {
                // Generate a random index between 0 and i (inclusive)
                var j = random.Next(i + 1);
            
                // Swap elements
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
