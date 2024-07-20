namespace WordCounter.Domain.SourceValidation;

public interface ISourceValidator
{
    Task<(ValidSource, IEnumerable<ValidationError>)> ValidationFile(string fileName, CancellationToken cancellationToken);
}