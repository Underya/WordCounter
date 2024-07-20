using System.Text;
using FluentAssertions;
using WordCounter.Domain.SourceValidation;
using WordCounter.Implementation.WordSource;

namespace WordCounter.Implementation.Tests.WordSource;

public class FileWordSourceTests
{
    private const string FileName = $"{nameof(FileWordSourceTests)}.txt";

    private const string Word1 = "word1";
    
    [TearDown]
    public void Clear()
    {
        TestsFileHelper.DeleteIfExists(FileName);
    }

    [Test]
    public async Task Create_SuccessCreate()
    {
        var filePath = await CreateFileAndGetPath(Word1);
        var fabric = CreateFabric();

        await using var wordSource = fabric.Create(new ValidSource(filePath));

        wordSource.Should().NotBeNull();
    }
    
    [Test]
    [Ignore("Early")]
    public async Task GetNextBatch_OneWord_Return()
    {
        var filePath = CreateFileAndGetPath(Word1);
    }

    private async Task<string> CreateFileAndGetPath(params string[] wordLines)
    {
        var builrder = new StringBuilder();
        foreach (var line in wordLines)
        {
            builrder.AppendLine(line);
        }

        return await TestsFileHelper.CreateFileAndGetPath(FileName, builrder.ToString());
    }

    private FileWordSourceFabric CreateFabric()
    {
        return new FileWordSourceFabric(new FileWordSourceOption());
    }
}