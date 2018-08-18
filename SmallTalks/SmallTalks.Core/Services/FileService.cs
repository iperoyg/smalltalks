using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmallTalks.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmallTalks.Core.Services
{
    public class FileService : IFileService
    {
        public RulesData ReadRulesFromFile(string file)
        {
            var dataAsString = File.ReadAllText(file, Encoding.UTF8);
            var stData = JsonConvert.DeserializeObject<RulesData>(dataAsString);
            return stData;
        }
    }
}
