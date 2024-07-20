namespace WordCounter.Domain;

public record WordCounterProcessorOption
{
    public int ThreadCount { get; init; }
};