using System.Data;
using Microsoft.Data.SqlClient;

namespace WordCounter.Implementation.WordCountSaver;

public class WordCountRepository : IWordCountRepository
{
    private const string TableName = @"[WordCounter].[WordCountStatistic]";

    private readonly IDbConnectionFactory _connectionFactory;

    public WordCountRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> WordExists(string word, CancellationToken cancellationToken)
    {
        var commandText = $"select id from {TableName} where [word] = @word";
        
        var result = await ExecuteReadScalarQuery(commandText, word, cancellationToken);

        return result is not null;
    }

    public async Task CreateNewWord(string word, int count, CancellationToken cancellationToken)
    {
        var commandText = $"insert into {TableName} ([word], [count]) VALUES (@word, @count)";
        
        await ExecuteEditQuery(commandText, word, count, cancellationToken);
    }

    public async Task IncreaseWordCount(string word, int count, CancellationToken cancellationToken)
    {
        var commandText = $"UPDATE {TableName} SET [count] = [count] + @count where [word] = @word";

        await ExecuteEditQuery(commandText, word, count, cancellationToken);
    }

    private async Task<object?> ExecuteReadScalarQuery(string query, string word, CancellationToken cancellationToken)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        
        var command = new SqlCommand(query, connection);
        command.Transaction = transaction as SqlTransaction;
        command.Parameters.Add(new SqlParameter("@word", word));
        
        return await command.ExecuteScalarAsync(cancellationToken);
    }

    private async Task ExecuteEditQuery(string query, string word, int count, CancellationToken cancellationToken)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        
        var command = new SqlCommand(query, connection);
        command.Transaction = transaction as SqlTransaction;

        command.Parameters.Add(new SqlParameter("@word", word));
        command.Parameters.Add(new SqlParameter("@count", count));
        
        await command.ExecuteNonQueryAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}