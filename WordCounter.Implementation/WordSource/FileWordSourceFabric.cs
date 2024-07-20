using WordCounter.Domain.SourceValidation;
using WordCounter.Domain.WordSource;

namespace WordCounter.Implementation.WordSource;

public class FileWordSourceFabric : IWordSourceFabric
{
    private readonly FileWordSourceOption _fileWordSourceOption;

    public FileWordSourceFabric(FileWordSourceOption fileWordSourceOption)
    {
        _fileWordSourceOption = fileWordSourceOption;
    }
    
    public IWordSource Create(ValidSource validSource)
    {
        var file = File.Open(validSource.FileName, FileMode.Open);
        return new FileWordSource(_fileWordSourceOption, file);
    }
}