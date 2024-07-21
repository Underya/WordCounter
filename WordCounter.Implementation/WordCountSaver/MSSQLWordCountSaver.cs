using Microsoft.Data.SqlClient;
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
        // Есть ненулевая вероятность,
        // что несколько разных копий приложения будут пытаться работать с одним и тем же словом одновременно
        // В этом случае лучше начинать транзакцию заного, т.к. данные могли быть изменены
        // (например, создана запись, которую мы пытались вставить) 
        
        await SqlRetryHelper.Retry(
            async () => await IncreaseCountInner(word, count, cancellationToken),
            cancellationToken);
    }

    private async Task IncreaseCountInner(string word, int count, CancellationToken cancellationToken)
    {
        var existValue = await _wordCountRepository.WordExists(word, cancellationToken);

        if (existValue)
        {
            await _wordCountRepository.IncreaseWordCount(word, count, cancellationToken);
        }
        else
        {
            await _wordCountRepository.CreateNewWord(word, count, cancellationToken);
        }
    }
}