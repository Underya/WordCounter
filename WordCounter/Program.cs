﻿using Microsoft.Extensions.DependencyInjection;
using WordCounter.Domain;
using WordCounter.Domain.SourceValidation;

namespace WordCounter;

internal class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = AddDependencyInjection();

        var fileName = args.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(fileName))
        {
            Console.WriteLine("FileName is empty or not set!");
            return;
        }
        
        await ProcessFile(serviceProvider, fileName);
    }

    private static ServiceProvider AddDependencyInjection()
    {
        var dICollection = new ServiceCollection();
        dICollection.AddWordCounterDependency();
        var serviceProvider = dICollection.BuildServiceProvider();
        return serviceProvider;
    }
    
    private static async Task ProcessFile(IServiceProvider serviceProvider, string fileName)
    {
        var validator = serviceProvider.GetService<ISourceValidator>();
        var logger = serviceProvider.GetService<ILogger>();
        var processor = serviceProvider.GetService<WordCounterProcessor>();
        
        var token = CancellationToken.None;
        
        var (file, errors) = await validator.ValidationFile(fileName, token);
        if (errors.Any())
        {
            logger.Log(errors, token);
            return;
        }

        await processor.Process(file!, token);
    }
}
