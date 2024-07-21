using WordCounter.Domain.SourceValidation;

namespace WordCounter.Domain;

public interface IWordValidator
{
    Task<IEnumerable<ValidationError>> ValidateWord(
        ValidSource source,
        string word,
        CancellationToken cancellationToken);
}