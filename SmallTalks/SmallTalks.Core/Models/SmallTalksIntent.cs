using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SmallTalks.Core.Models
{
    public class SmallTalksIntent
    {
        public string Name { get; set; }
        public Regex Regex { get; set; }
        public int Priority { get; set; }
    }
}
