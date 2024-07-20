using WordCounter.Domain.SourceValidation;

namespace WordCounter.Domain;

public interface ILogger
{
     Task Log(ValidSource source, IEnumerable<ValidationError> validationErrors, CancellationToken cancellationToken);
}