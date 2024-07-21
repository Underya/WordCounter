using System.Data;
using Microsoft.Data.SqlClient;

namespace WordCounter.Implementation.WordCountSaver;

public interface IDbConnectionFactory
{
    SqlConnection CreateConnection();
}