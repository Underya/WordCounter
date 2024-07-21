using SimpleMigrations;

namespace WordCounter.Implementation.DataBaseMigration;

[Migration(1, "Create schema and table")]
public class InitializeMigration : Migration
{
    protected override void Up()
    {
        Execute(@"
IF (NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'WordCounter')) 
BEGIN
    EXEC ('CREATE SCHEMA WordCounter');
END;
");
    
        Execute(@"
IF NOT EXISTS 
	(SELECT * FROM sysobjects WHERE name='WordCountStatistic' and xtype='U')
BEGIN
	CREATE TABLE [WordCounter].[WordCountStatistic] (
        [Id] BIGINT NOT NULL IDENTITY(1,1),
        [Word] nvarchar(20) NOT NULL,
        [Count] int NOT NULL
    );
   
   CREATE UNIQUE INDEX [Uniq_Word_WithCount_WodrCountStatistic]
   ON [master].[WordCounter].[WordCountStatistic] ([Word])
   INCLUDE ([Count]);
END
");
    }

    protected override void Down()
    {
        
    }
}
