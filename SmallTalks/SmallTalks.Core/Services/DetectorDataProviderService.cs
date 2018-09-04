using SmallTalks.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallTalks.Core.Services
{
    public class DetectorDataProviderService : IDetectorDataProviderService
    {
        private readonly IFileService _fileService;
        private readonly IConversionService _conversionService;

        public DetectorDataProviderService(IFileService fileService, IConversionService conversionService)
        {
            _fileService = fileService;
            _conversionService = conversionService;
        }

        public SmallTalksDectectorData GetSmallTalksDetectorDataFromSource(SourceProvider source)
        {
            var rules = _fileService.ReadRulesFromFile(source.Intents);
            var data = _conversionService.ToDetectorData(rules);
            return data;
        }
    }
}
