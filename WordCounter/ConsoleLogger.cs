using WordCounter.Domain;
using WordCounter.Domain.SourceValidation;

namespace WordCounter;

public class ConsoleLogger : ILogger
{
    public Task Log(IEnumerable<ValidationError> validationErrors, CancellationToken cancellationToken)
    {
        foreach (var error in validationErrors)
            Console.WriteLine($"{error.FileName}:{error.ValidationMessage}");
        
        return Task.CompletedTask;
    }
}