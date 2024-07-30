using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Utils
{
    public static class CollectionsExtensions
    {
        /// <summary>
        /// Returns all items in the first collection except the ones in the second collection that match the lambda condition
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="listA">The first list</param>
        /// <param name="listB">The second list</param>
        /// <param name="lambda">The filter expression</param>
        /// <returns>The filtered list</returns>
        public static IEnumerable<T> LExcept<T>(this IEnumerable<T> listA, IEnumerable<T> listB, Func<T, T, bool> lambda)
        {
            return listA.Except(listB, new LambdaComparer<T>(lambda));
        }

        /// <summary>
        /// Returns all items in the first collection that intersect the ones in the second collection that match the lambda condition
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="listA">The first list</param>
        /// <param name="listB">The second list</param>
        /// <param name="lambda">The filter expression</param>
        /// <returns>The filtered list</returns>
        public static IEnumerable<T> LIntersect<T>(this IEnumerable<T> listA, IEnumerable<T> listB, Func<T, T, bool> lambda)
        {
            return listA.Intersect(listB, new LambdaComparer<T>(lambda));
        }
    }
    
    public class LambdaComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _expression;

        public LambdaComparer(Func<T, T, bool> lambda)
        {
            _expression = lambda;
        }

        public bool Equals(T x, T y)
        {
            return _expression(x, y);
        }

        public int GetHashCode(T obj)
        {
            /*
             If you just return 0 for the hash the Equals comparer will kick in. 
             The underlying evaluation checks the hash and then short circuits the evaluation if it is false.
             Otherwise, it checks the Equals. If you force the hash to be true (by assuming 0 for both objects), 
             you will always fall through to the Equals check which is what we are always going for.
            */
            return 0;
        }
    }
}