using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmallTalks.Core.Services.Interfaces
{
    public interface IWordDetectorFactory
    {
        Task<IWordsDetector> GetWordsDetectorAsync(WordDetectorType type);
    }

    public enum WordDetectorType
    {
        Stopwords,
        Cursewords
    }
}

