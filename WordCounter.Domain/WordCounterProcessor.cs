using WordCounter.Domain.SourceValidation;
using WordCounter.Domain.WordSource;

namespace WordCounter.Domain;

public class WordCounterProcessor
{
    private int CountThreads = 5;
    
    private readonly IWordSourceFabric _wordSourceFabric;
    private readonly IWordCountSaver _wordCountSaver;
    private readonly IWordValidator _wordValidator;
    private readonly ILogger _logger;

    public WordCounterProcessor(
        IWordSourceFabric wordSourceFabric,
        IWordCountSaver wordCountSaver,
        IWordValidator wordValidator,
        ILogger logger)
    {
        _wordSourceFabric = wordSourceFabric;
        _wordCountSaver = wordCountSaver;
        _wordValidator = wordValidator;
        _logger = logger;
    }
    
    public async Task Process(ValidSource validSource, CancellationToken cancellationToken)
    {
        using var wordSource = _wordSourceFabric.Create(validSource);

        while (true)
        {
            var wordBatch = await wordSource.GetNextBatch(cancellationToken);

            if (!wordBatch.Any())
                break;

            var groupedWordBatch = wordBatch
                .GroupBy(word => word)
                .Select(groupedWord => new GroupedWord(groupedWord.Key, groupedWord.Count()));

            await Parallel.ForEachAsync(
                groupedWordBatch,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = CountThreads,
                    CancellationToken = cancellationToken
                },
                async (groupedWord, token) => await ProcessGroupedWord(groupedWord, token));
             
        }
    }

    private async Task ProcessGroupedWord(GroupedWord groupedWord, CancellationToken cancellationToken)
    {
        var validationErrors = await _wordValidator.ValidWord(groupedWord.Word, cancellationToken);
        if (validationErrors.Any())
        {
            await _logger.Log(validationErrors, cancellationToken);
            return;
        }

        await _wordCountSaver.IncreaseWordCount(groupedWord.Word, groupedWord.Count, cancellationToken);
    }

    private record GroupedWord(string Word, int Count);
}