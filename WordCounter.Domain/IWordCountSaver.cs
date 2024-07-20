namespace WordCounter.Domain;

public interface IWordCountSaver
{
    Task IncreaseWordCount(string word, int count, CancellationToken cancellationToken);
}