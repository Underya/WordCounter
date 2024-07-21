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
        
        var task1 = ProcessAsync(serviceProvider, "test.txt");
        var task2 = ProcessAsync(serviceProvider, "test2.txt");
        var task3 = ProcessAsync(serviceProvider, "test3.txt");

        Task.WaitAll(task1, task2, task3);
    }

    private static async Task ProcessAsync(ServiceProvider serviceProvider, string filaName)
    {
        var validator = serviceProvider.GetService<ISourceValidator>();
        var logger = serviceProvider.GetService<ILogger>();
        var processor = serviceProvider.GetService<WordCounterProcessor>();

        await Task.Delay(TimeSpan.FromSeconds(1));
        
        var token = CancellationToken.None;

        var (file, errors) = await validator.ValidationFile(filaName, token);
        if (errors.Any())
        {
            logger.Log(errors, token);
            return;
        }

        await processor.Process(file!, token);
    }
}
