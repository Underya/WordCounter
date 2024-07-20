namespace WordCounter.Domain;

public interface IWordValidator
{
    Task<IEnumerable<ValidationError>> ValidWord(string word);
}