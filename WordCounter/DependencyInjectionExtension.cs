
using Microsoft.Extensions.DependencyInjection;
using WordCounter.Domain;
using WordCounter.Domain.SourceValidation;
using WordCounter.Domain.WordCountSaver;
using WordCounter.Domain.WordSource;
using WordCounter.Implementation;
using WordCounter.Implementation.SourceValidation;
using WordCounter.Implementation.WordCountSaver;
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
            ThreadCount = 1
        });

        collection.AddScoped<ILogger, ConsoleLogger>();

        collection.AddScoped<IWordCountSaver, MSSQLWordCountSaver>();
        collection.AddScoped<IWordCountRepository, WordCountRepository>();
        collection.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        collection.AddScoped(_ => new DbConnectionOption
        {
            ConnectionString = @"Server=localhost,1433;Database=master;User Id=sa;Password=TestsPassword@123;TrustServerCertificate=True"
        });
    }
}