namespace WordCounter.Implementation.WordSource;

public record FileWordSourceOption
{
    public int WordsInBatch { get; init; }
    public int CountSymbolRead { get; init; }
};