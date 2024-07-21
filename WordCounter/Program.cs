using Microsoft.Extensions.DependencyInjection;
using WordCounter.Domain;
using WordCounter.Domain.SourceValidation;

namespace WordCounter;

internal class Program
{
    static async Task Main(string[] args)
    {
        var dICollection = new ServiceCollection();
        dICollection.AddWordCounterDependency();

        var serviceProvider = dICollection.BuildServiceProvider();
        var validator = serviceProvider.GetService<ISourceValidator>();
        var logger = serviceProvider.GetService<ILogger>();
        var processor = serviceProvider.GetService<WordCounterProcessor>();
        
        var token = CancellationToken.None;
        
        var (file, errors) = await validator.ValidationFile("test.txt", token);
        if (errors.Any())
        {
            logger.Log(errors, token);
            return;
        }

        await processor.Process(file!, token);

    }
}
