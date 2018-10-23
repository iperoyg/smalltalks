using System;
using System.Collections.Generic;
using System.Text;

namespace SmallTalks.Core.Models
{
    public class SmallTalksPreProcessingConfiguration
    {
        public bool UnicodeNormalization { get; set; }
        public bool ToLower { get; set; }

        public SmallTalksPreProcessingConfiguration()
        {
            UnicodeNormalization = false;
            ToLower = true;
        }
    }
}
