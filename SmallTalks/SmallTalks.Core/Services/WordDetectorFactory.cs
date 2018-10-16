using SmallTalks.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmallTalks.Core.Services
{
    public class WordDetectorFactory : IWordDetectorFactory
    {
        private static IWordsDetector _stopwordsDetector = null;
        private static IWordsDetector _cursewordsDetector = null;

        private readonly IServiceProvider _serviceProvider;
        private readonly ISourceProviderService _sourceProvider;

        public WordDetectorFactory(
            IServiceProvider serviceProvider,
            ISourceProviderService sourceProvider)
        {
            _serviceProvider = serviceProvider;
            _sourceProvider = sourceProvider;
        }

        public async Task<IWordsDetector> GetWordsDetectorAsync(WordDetectorType type)
        {
            var wordDetector = (IWordsDetector)_serviceProvider.GetService(typeof(IWordsDetector));
            switch (type)
            {
                case WordDetectorType.Stopwords:
                    if(_stopwordsDetector == null)
                    {
                        await wordDetector.LoadAsync(_sourceProvider.GetSourceProvider().StopWords);
                        _stopwordsDetector = wordDetector;
                    }
                    return _stopwordsDetector;
                case WordDetectorType.Cursewords:
                    if (_cursewordsDetector == null)
                    {
                        await wordDetector.LoadAsync(_sourceProvider.GetSourceProvider().CurseWords);
                        _cursewordsDetector = wordDetector;
                    }
                    return _cursewordsDetector;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
