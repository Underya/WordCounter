using WordCounter.Domain.SourceValidation;

namespace WordCounter.Domain.WordSource;

public interface IWordSourceFabric
{
    IWordSource Create(ValidSource validSource);
}