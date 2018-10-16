using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmallTalks.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmallTalks.Core.Services
{
    public class FileService : IFileService
    {
        public async Task<RulesData> ReadRulesFromSourceAsync(string file)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var intentsStream = assembly.GetManifestResourceStream($"SmallTalks.Core.{file}");
            //var stopWordsStream = assembly.GetManifestResourceStream("SmallTalks.Core.Resources.stopwords.txt");
            using (var reader = new StreamReader(intentsStream, Encoding.UTF8))
            {
                var dataAsString = await reader.ReadToEndAsync();
                var stData = JsonConvert.DeserializeObject<RulesData>(dataAsString);
                return stData;
            }
            
        }
    }
}
