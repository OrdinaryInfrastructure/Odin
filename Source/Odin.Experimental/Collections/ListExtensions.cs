using System;
using System.Collections.Generic;

namespace Odin.Collections
{
    /// <summary>
    /// For System.Collections.Generic.List
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Splits a collections in several collections with a smaller size
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="smallerListSize"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<List<T>> SplitIntoListsOfSize<T>(this List<T> collection, int smallerListSize)  
        {        
            for (int i = 0; i < collection.Count; i += smallerListSize) 
            { 
                yield return collection.GetRange(i, Math.Min(smallerListSize, collection.Count - i)); 
            }  
        } 
    }
}