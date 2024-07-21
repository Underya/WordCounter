namespace WordCounter.Implementation.WordCountSaver;

public interface IWordCountRepository
{
    Task<bool> WordExists(string word, CancellationToken cancellationToken);

    Task CreateNewWord(string word, int count, CancellationToken cancellationToken);

    Task IncreaseWordCount(string word, int count, CancellationToken cancellationToken);
}