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
            return (source.FirstIndexOf(pattern) != -1);

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
            if (pattern == null) throw new ArgumentNullException();

            int patternLength = pattern.Length;
            int totalLength = source.Count();
            TSource firstMatch = pattern[0];

            if (firstMatch == null) return -1;

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

        /// <summary>
        /// Maps a source value's position within a range of numbers to the same positon
        /// within a another range of numbers. For instance, will map a source value of `30`
        /// in the range of `0` to `100` to a value of `0.3` in a given range of `0.0` to `1.0`.
        /// </summary>
        /// <param name="souceValue">The value to map to the new domain.</param>
        /// <param name="sourceMin">The minimum value of the source domain.</param>
        /// <param name="sourceMax">The maximum value of the source domain.</param>
        /// <param name="targetMin">The minimum value of the destinatino domain.</param>
        /// <param name="targetMax">The maximum value of the destination domain.</param>
        /// <returns></returns>
        static public float Map(this float souceValue, float sourceMin, float sourceMax, float targetMin, float targetMax)
        {
            return (souceValue - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin) + targetMin;
        }

        /// <summary>
        /// Maps a source value's position within a range of numbers to the same positon
        /// within a another range of numbers. For instance, will map a source value of `30`
        /// in the range of `0` to `100` to a value of `0.3` in a given range of `0.0` to `1.0`.
        /// </summary>
        /// <param name="souceValue">The value to map to the new domain.</param>
        /// <param name="sourceMin">The minimum value of the source domain.</param>
        /// <param name="sourceMax">The maximum value of the source domain.</param>
        /// <param name="targetMin">The minimum value of the destinatino domain.</param>
        /// <param name="targetMax">The maximum value of the destination domain.</param>
        /// <returns></returns>
        static public double Map(this double souceValue, double sourceMin, double sourceMax, double targetMin, double targetMax)
        {
            return (souceValue - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin) + targetMin;
        }

        /// <summary>
        /// Maps a source value's position within a range of numbers to the same positon
        /// within a another range of numbers. For instance, will map a source value of `30`
        /// in the range of `0` to `100` to a value of `0.3` in a given range of `0.0` to `1.0`.
        /// </summary>
        /// <param name="souceValue">The value to map to the new domain.</param>
        /// <param name="sourceMin">The minimum value of the source domain.</param>
        /// <param name="sourceMax">The maximum value of the source domain.</param>
        /// <param name="targetMin">The minimum value of the destinatino domain.</param>
        /// <param name="targetMax">The maximum value of the destination domain.</param>
        /// <returns></returns>
        static public int Map(this int souceValue, int sourceMin, int sourceMax, int targetMin, int targetMax)
        {
            return (souceValue - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin) + targetMin;
        }
    }
}
