﻿using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SocialWave.Extensions
{
    /// <summary>
    /// Provides extension methods for string manipulation.
    /// </summary>
    static class StringExtension
    {
        /// <summary>
        /// Cuts the string to a maximum length of 10 characters and appends "..." if the original string exceeds this length.
        /// </summary>
        /// <param name="thisString">The input string.</param>
        /// <returns>The modified string.</returns>
        public static string CutName(this string thisString)
        {
            if (thisString.Length <= 10)
            {
                return thisString;
            }
            else
            {
                return thisString.Substring(0, 10) + "...";
            }
        }

        /// <summary>
        /// Cuts the string to a maximum length of 300 characters and appends "..." if the original string exceeds this length.
        /// </summary>
        /// <param name="thisString">The input string.</param>
        /// <returns>The modified string.</returns>
        public static string CutDescription(this string thisString)
        {
            if(thisString == null) 
            {
                return " ";
            }
            else if (thisString.Length <= 500)
            {
                return thisString;
            }
            else
            {
                return thisString.Substring(0, 500) + "...";
            }
        }

        /// <summary>
        /// Provides an extension method for cutting a string to a maximum length of 100 characters.
        /// </summary>
        /// <param name="thisString">The string instance to be cut.</param>
        /// <returns>
        /// Returns the original string if its length is less than or equal to 100 characters; otherwise, returns a truncated string
        /// with the first 100 characters followed by an ellipsis ("...").
        /// </returns>
        public static string CutComment(this string thisString)
        {
            if (thisString.Length <= 100)
            {
                return thisString;
            }
            else
            {
                return thisString.Substring(0, 100) + "...";
            }
        }
    }
}
