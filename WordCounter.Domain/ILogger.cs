namespace WordCounter.Domain;

public interface ILogger
{
     Task Log(IEnumerable<ValidationError> validationErrors, CancellationToken cancellationToken);
}