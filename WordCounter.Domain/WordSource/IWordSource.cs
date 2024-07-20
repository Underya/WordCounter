namespace WordCounter.Domain.WordSource;

public interface IWordSource : IDisposable, IAsyncDisposable
{
    Task<IEnumerable<string>> GetNextBatch(CancellationToken cancellationToken);
}