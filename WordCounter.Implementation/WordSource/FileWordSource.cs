using WordCounter.Domain.WordSource;

namespace WordCounter.Implementation.WordSource;

public class FileWordSource : IWordSource
{
    private readonly FileWordSourceOption _wordSourceOption;
    private readonly FileStream _fileStream;

    public FileWordSource(FileWordSourceOption wordSourceOption, FileStream fileStream)
    {
        _wordSourceOption = wordSourceOption;
        _fileStream = fileStream;
    }
    
    public Task<IEnumerable<string>> GetNextBatch(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        _fileStream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _fileStream.DisposeAsync();
    }
}