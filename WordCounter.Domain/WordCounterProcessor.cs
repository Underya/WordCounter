using WordCounter.Domain.SourceValidation;
using WordCounter.Domain.WordSource;

namespace WordCounter.Domain;

public class WordCounterProcessor
{
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
    
    public async Task Process(ValidSource validSource)
    {
        using var wordSource = _wordSourceFabric.Create(validSource);

        while (true)
        {
            var wordBatch = await wordSource.GetNextBatch();

            if (!wordBatch.Any())
                break;

            var groupedWordBatch = wordBatch
                .GroupBy(word => word)
                .Select(groupedWord => new GroupedWord(groupedWord.Key, groupedWord.Count()));

            foreach (var groupedWord in groupedWordBatch)
                await ProcessGroupedWord(groupedWord);
        }
    }

    private async Task ProcessGroupedWord(GroupedWord groupedWord)
    {
        var validationErrors = await _wordValidator.ValidWord(groupedWord.Word);
        if (validationErrors.Any())
        {
            await _logger.Log(validationErrors);
            return;
        }

        await _wordCountSaver.IncreaseWordCount(groupedWord.Word, groupedWord.Count);
    }

    private record GroupedWord(string Word, int Count);
}