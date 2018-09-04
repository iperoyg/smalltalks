using System;
using System.Collections.Generic;
using System.Text;

namespace SmallTalks.Core.Models
{
    public class SourceProvider
    {
        public string Intents { get; set; }
        public string StopWords { get; set; }
        public SourceProviderType SourceType { get; set; }
    }

    public enum SourceProviderType
    {
        Local,
        Web
    }
}
