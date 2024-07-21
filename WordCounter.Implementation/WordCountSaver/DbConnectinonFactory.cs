using System.Data;
using Microsoft.Data.SqlClient;

namespace WordCounter.Implementation.WordCountSaver;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly DbConnectionOption _connectionOption;

    public DbConnectionFactory(DbConnectionOption connectionOption)
    {
        _connectionOption = connectionOption;
    }
    
    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionOption.ConnectionString);
    }
}