using SmallTalks.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallTalks.Core.Services
{
    public class LocalSourceProviderService : ISourceProviderService
    {
        public SourceProvider GetSourceProvider()
        {
            return new SourceProvider { Source = "intents.json" };
        }
    }
}
