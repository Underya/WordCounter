using WordCounter.Domain.WordCountSaver;

namespace WordCounter.Implementation.WordCountSaver;

public class MSSQLWordCountSaver : IWordCountSaver
{
    private readonly IWordCountRepository _wordCountRepository;

    public MSSQLWordCountSaver(IWordCountRepository wordCountRepository)
    {
        _wordCountRepository = wordCountRepository;
    }
    
    public async Task IncreaseWordCount(string word, int count, CancellationToken cancellationToken)
    {
        var existValue = await _wordCountRepository.WordExists(word, cancellationToken);

        if (!existValue)
        {
            await _wordCountRepository.CreateNewWord(word, count, cancellationToken);
        }
        else
        {
            await _wordCountRepository.IncreaseWordCount(word, count, cancellationToken);
        }
    }
}