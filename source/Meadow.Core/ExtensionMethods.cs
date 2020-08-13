using System;
using System.Linq;
using System.Collections.Generic;

namespace Meadow
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// TODO: move this into the `CircularBuffer` class? or is it broadly applicable?
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        static public bool Contains<TSource>(this IEnumerable<TSource> source, TSource[] pattern)
        {
            return (source.FirstIndexOf(pattern) == -1 ? false : true);

            //int patternLength = pattern.Length;
            //int totalLength = source.Count();
            //TSource firstMatch = pattern[0];

            //for (int i = 0; i < totalLength; i++) {
            //    // is this the right equality?
            //    if ( (firstMatch.Equals (source.ElementAt(i)) ) // begin match?
            //         &&
            //         (totalLength - i >= patternLength) // can match exist?
            //       ) {
            //        TSource[] matchTest = new TSource[patternLength];
            //        // copy the potential match into the matchTest array.
            //        // can't use .Skip() and .Take() because it will actually
            //        // enumerate over stuff and can have side effects
            //        for (int x = 0; x < patternLength; x++) {
            //            matchTest[x] = source.ElementAt(i + x);
            //        }
            //        // if the pattern pulled from source matches our search pattern
            //        // then the pattern exists.
            //        if (matchTest.SequenceEqual(pattern)) {
            //            return true;
            //        }
            //    }
            //}
            //// if we go here, no pattern match
            //return false;
        }

        /// TODO: move this into the `CircularBuffer` class? or is it broadly applicable?
        static public int FirstIndexOf<TSource>(this IEnumerable<TSource> source, TSource[] pattern)
        {
            int patternLength = pattern.Length;
            int totalLength = source.Count();
            TSource firstMatch = pattern[0];

            for (int i = 0; i < totalLength; i++) {
                // is this the right equality?
                if ((firstMatch.Equals(source.ElementAt(i))) // begin match?
                     &&
                     (totalLength - i >= patternLength) // can match exist?
                   ) {
                    TSource[] matchTest = new TSource[patternLength];
                    // copy the potential match into the matchTest array.
                    // can't use .Skip() and .Take() because it will actually
                    // enumerate over stuff and can have side effects
                    for (int x = 0; x < patternLength; x++) {
                        matchTest[x] = source.ElementAt(i + x);
                    }
                    // if the pattern pulled from source matches our search pattern
                    // then the pattern exists.
                    if (matchTest.SequenceEqual(pattern)) {
                        return i;
                    }
                }
            }
            // if we go here, doesn't exist
            return -1;
        }
    }
}
