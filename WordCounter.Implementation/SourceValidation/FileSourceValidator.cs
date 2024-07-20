using WordCounter.Domain;
using WordCounter.Domain.SourceValidation;

namespace WordCounter.Implementation.SourceValidation;

public class FileSourceValidator : ISourceValidator
{
    public async Task<(ValidSource?, IEnumerable<ValidationError>)> ValidationFile(
        string fileName,
        CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        if (!File.Exists(fullPath))
        {
            var error = new ValidationError(fileName, $"file not exists. Full path {fullPath}");
            return (null, new[] { error });
        }

        return (new ValidSource(fullPath), Array.Empty<ValidationError>());
    }
}