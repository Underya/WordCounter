using WordCounter.Domain.SourceValidation;

namespace WordCounter.Domain;

public interface ILogger
{
     Task Log(IEnumerable<ValidationError> validationErrors, CancellationToken cancellationToken);
}