﻿using SmallTalks.Core.Models;
using SmallTalks.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<SmallTalksDectectorData> GetSmallTalksDetectorDataFromSourceAsync(SourceProvider source)
        {
            var rules = await _fileService.ReadRulesFromSourceAsync(source.Intents);
            var data = _conversionService.ToDetectorData(rules);
            return data;
        }

        public async Task<SmallTalksDectectorData> GetSmallTalksDetectorDataFromSourceAsyncv2(SourceProvider source)
        {
            var rulesv2 = await _fileService.ReadRulesFromSourceAsync(source.Intents);
            var datav2 = _conversionService.ToDetectorDatav2(rulesv2);
            return datav2;
        }
    }
}
