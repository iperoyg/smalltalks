using System;
using System.Collections.Generic;
using System.Text;

namespace SmallTalks.Core.Models
{
    public class SourceProvider
    {
        public string Source { get; set; }
        public SourceProviderType SourceType { get; set; }
    }

    public enum SourceProviderType
    {
        Local
    }
}
