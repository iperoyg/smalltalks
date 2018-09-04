namespace SmallTalks.Core
{
    public interface IStopWordsDetector
    {
        bool HaveStopWords(string input);
        string RemoveStopWords(string input);
    }
}