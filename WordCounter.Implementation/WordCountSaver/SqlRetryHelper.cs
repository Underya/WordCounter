using Microsoft.Data.SqlClient;

namespace WordCounter.Implementation.WordCountSaver;

public static class SqlRetryHelper
{
    private const int MaxRetryCount = 5;
    private static readonly TimeSpan StartDelay = TimeSpan.FromMilliseconds(5);
    private static readonly TimeSpan IncreaseDelay = TimeSpan.FromMilliseconds(5);
    private const int MinJitterMilliSecond = 2;
    private const int MaxJitterMilliSecond = 10;
    
    public static async Task Retry(Func<Task> action, CancellationToken cancellationToken)
    {
         
        
        var currentTry = 0;
        var currentDelay = StartDelay;
        var jitterGenerator = new Random();
        
        while (true)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                    
                await action();
                break;
            }
            catch (Exception exception)
            {
                if (!ExceptionRetryable(exception))
                    throw;

                currentTry++;
                if (MaxRetryCount <= currentTry)
                    throw;

                cancellationToken.ThrowIfCancellationRequested();
                
                await Task.Delay(currentDelay, cancellationToken);

                var jitter = jitterGenerator.Next(MinJitterMilliSecond, MaxJitterMilliSecond);
                currentDelay += IncreaseDelay + TimeSpan.FromMilliseconds(jitter);
            }
        }
    }
    
    private static bool ExceptionRetryable(Exception exc)
    {
        // Тут конечно не должно быть такой обработки ошибок
        // Тут далее необходимо выбрать некоторый набор конкретных ошибок, типа дедлоков, конфликтов изменения
        // таймаутов и т.д. 
        // Но думаю, что в рамках тестового задания этого хватит
        return exc is SqlException;
    }
}