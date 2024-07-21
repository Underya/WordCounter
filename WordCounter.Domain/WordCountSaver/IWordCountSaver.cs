namespace WordCounter.Domain.WordCountSaver;

public interface IWordCountSaver
{
    Task IncreaseWordCount(string word, int count, CancellationToken cancellationToken);
}