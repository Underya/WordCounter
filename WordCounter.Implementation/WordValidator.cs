using WordCounter.Domain;
using WordCounter.Domain.SourceValidation;

namespace WordCounter.Implementation;

public class WordValidator : IWordValidator
{
    private const int MinWordLenght = 3;
    private const int MaxWordLenght = 20;
    
    public Task<IEnumerable<ValidationError>> ValidateWord(
        ValidSource source,
        string word,
        CancellationToken cancellationToken)
    {
        if (word.Length < MinWordLenght)
        {
            var error = new ValidationError(
                source.FileName,
                $"word:{word} shorter than minimum value {MinWordLenght}");
            return Task.FromResult<IEnumerable<ValidationError>>([error]);
        }

        if (MaxWordLenght < word.Length)
        {
            var error = new ValidationError(
                source.FileName,
                $"word:{word} longer than maximum value {MaxWordLenght}");
            return Task.FromResult<IEnumerable<ValidationError>>([error]);
        }
        
        return Task.FromResult<IEnumerable<ValidationError>>([]);
    }
}