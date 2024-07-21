namespace WordCounter.Implementation.WordCountSaver;

public record DbConnectionOption
{
    public string ConnectionString { get; init; }
};