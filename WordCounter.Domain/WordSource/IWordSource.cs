namespace WordCounter.Domain.WordSource;

public interface IWordSource : IDisposable
{
    Task<IEnumerable<string>> GetNextBatch(CancellationToken cancellationToken);
}