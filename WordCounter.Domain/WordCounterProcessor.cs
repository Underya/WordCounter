using WordCounter.Domain.SourceValidation;
using WordCounter.Domain.WordCountSaver;
using WordCounter.Domain.WordSource;

namespace WordCounter.Domain;

public class WordCounterProcessor
{
    private readonly int _countThreads;
    private readonly IWordSourceFabric _wordSourceFabric;
    private readonly IWordCountSaver _wordCountSaver;
    private readonly IWordValidator _wordValidator;
    private readonly ILogger _logger;

    public WordCounterProcessor(
        WordCounterProcessorOption wordCounterProcessorOption,
        IWordSourceFabric wordSourceFabric,
        IWordCountSaver wordCountSaver,
        IWordValidator wordValidator,
        ILogger logger)
    {
        _countThreads = wordCounterProcessorOption.ThreadCount;
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
                    MaxDegreeOfParallelism = _countThreads,
                    CancellationToken = cancellationToken
                },
                async (groupedWord, token) => await ProcessGroupedWord(validSource, groupedWord, token));
             
        }
    }

    private async Task ProcessGroupedWord(ValidSource validSource, GroupedWord groupedWord, CancellationToken cancellationToken)
    {
        var validationErrors = await _wordValidator.ValidateWord(validSource, groupedWord.Word, cancellationToken);
        if (validationErrors.Any())
        {
            await _logger.Log(validationErrors, cancellationToken);
            return;
        }

        await _wordCountSaver.IncreaseWordCount(groupedWord.Word, groupedWord.Count, cancellationToken);
    }

    private record GroupedWord(string Word, int Count);
}