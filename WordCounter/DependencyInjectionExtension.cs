
using Microsoft.Extensions.DependencyInjection;
using WordCounter.Domain;
using WordCounter.Domain.SourceValidation;
using WordCounter.Domain.WordSource;
using WordCounter.Implementation;
using WordCounter.Implementation.SourceValidation;
using WordCounter.Implementation.WordSource;

namespace WordCounter;

public static class DependencyInjectionExtension
{
    public static void AddWordCounterDependency(this IServiceCollection collection)
    {
        collection.AddScoped<ISourceValidator, FileSourceValidator>();
        collection.AddScoped<IWordSourceFabric, FileWordSourceFabric>();
        collection.AddScoped(_ => new FileWordSourceOption
        {
            WordsInBatch = 1000,
            CountSymbolRead = 200
        });

        collection.AddScoped<IWordValidator, WordValidator>();
        
        collection.AddTransient<WordCounterProcessor>();
        collection.AddScoped(_ => new WordCounterProcessorOption
        {
            ThreadCount = 5
        });

        collection.AddScoped<IWordCountSaver, DbSaveStub>();
        collection.AddScoped<ILogger, ConsoleLogger>();
        
    }
}

public class DbSaveStub : IWordCountSaver
{
    public Task IncreaseWordCount(string word, int count, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Word:{word}. Count:{count}");
        return Task.CompletedTask;
    }
}