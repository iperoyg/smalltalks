using System;
using System.Collections.Generic;
using System.Text;

namespace SmallTalks.Core
{
    //Credit to: https://codereview.stackexchange.com/questions/119519/regex-to-first-match-then-replace-found-matches
    public static class StringExtension
    {
        public static string Replace(this string s, int index, int length, string replacement)
        {
            var builder = new StringBuilder();
            builder.Append(s.Substring(0, index));
            builder.Append(replacement);
            builder.Append(s.Substring(index + length));
            return builder.ToString();
        }

        public static string Replace(this string s, int index, int length, char replacement)
        {
            var builder = new StringBuilder();
            builder.Append(s.Substring(0, index));
            builder.Append(replacement, length);
            builder.Append(s.Substring(index + length));
            return builder.ToString();
        }
    }

}
