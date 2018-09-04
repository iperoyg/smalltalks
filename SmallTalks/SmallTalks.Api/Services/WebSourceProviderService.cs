using SmallTalks.Core.Models;
using SmallTalks.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmallTalks.Api.Services
{
    public class WebSourceProviderService : ISourceProviderService
    {
        public SourceProvider GetSourceProvider()
        {
            var intentsFile = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "intents.json");
            var swFile = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "stopwords.txt");
            return new SourceProvider { Intents = intentsFile, StopWords = swFile, SourceType = SourceProviderType.Web };
        }
    }
}
