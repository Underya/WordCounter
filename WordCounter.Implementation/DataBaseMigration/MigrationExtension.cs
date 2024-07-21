using System.Reflection;
using SimpleMigrations;
using SimpleMigrations.DatabaseProvider;
using WordCounter.Implementation.WordCountSaver;

namespace WordCounter.Implementation.DataBaseMigration;

public static class MigrationExtension
{
    public static void DoMigration(this IServiceProvider serviceProvider)
    {
        var assembly = typeof(MigrationExtension).Assembly;
        var connectionFactory = serviceProvider.GetService(typeof(IDbConnectionFactory)) as IDbConnectionFactory;
        using var connection = connectionFactory.CreateConnection();
        var provider = new MssqlDatabaseProvider(connection);
        var migration = new SimpleMigrator(assembly, provider);
        
        migration.Load();
        migration.MigrateToLatest();
    }
}