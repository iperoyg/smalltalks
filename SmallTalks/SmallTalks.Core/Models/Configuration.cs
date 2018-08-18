using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SmallTalks.Core.Models
{
    public static class Configuration
    {
        public const RegexOptions ST_REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline;
    }
}
