
using Microsoft.Extensions.Configuration;
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
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        collection.AddScoped<IConfigurationRoot>(_ => config);
        
        collection.AddScoped<ISourceValidator, FileSourceValidator>();
        collection.AddScoped<IWordSourceFabric, FileWordSourceFabric>();
        collection.AddScoped(serviceProvider =>
            serviceProvider
                .GetService<IConfigurationRoot>()!
                .GetSection("WordSourceOption")
                .Get<FileWordSourceOption>()
        );
        collection.AddScoped<IWordValidator, WordValidator>();
        
        collection.AddTransient<WordCounterProcessor>();
        collection.AddScoped(serviceProvider =>
            serviceProvider
                .GetService<IConfigurationRoot>()!
                .GetSection("WordCounterOption")
                .Get<WordCounterProcessorOption>()
        );

        collection.AddScoped<ILogger, ConsoleLogger>();

        collection.AddScoped<IWordCountSaver, MSSQLWordCountSaver>();
        collection.AddScoped<IWordCountRepository, WordCountRepository>();
        collection.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        collection.AddScoped(serviceProvider =>
            serviceProvider
                .GetService<IConfigurationRoot>()!
                .GetSection("DataBaseConnectionOption")
                .Get<DbConnectionOption>()
        );
    }
}