using System.Text;
using FluentAssertions;
using WordCounter.Domain.SourceValidation;
using WordCounter.Implementation.WordSource;

namespace WordCounter.Implementation.Tests.WordSource;

public class FileWordSourceTests
{
    private const string FileName = $"{nameof(FileWordSourceTests)}.txt";

    private const string Word1 = "word";
    private const string Word2 = "words";
    private const string WordRuLocal1 = "слово";
    private const string WordRuLocal2 = "слова";
    
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
    public async Task GetNextBatch_OneWord_Return()
    {
        var filePath = await CreateFileAndGetPath($"{Word1}");
        var fabric = CreateFabric();
        await using var wordSource = fabric.Create(new ValidSource(filePath));

        var wordBatch = await wordSource.GetNextBatch(CancellationToken.None);

        wordBatch.Should().ContainSingle()
            .And.Contain(word => word == Word1);
    }

    [Test]
    public async Task GetNextBatch_SomeWords_Return()
    {
        var filePath = await CreateFileAndGetPath(
            $"{Word1} {Word2}",
            $"{WordRuLocal1} {WordRuLocal2}");
        var fabric = CreateFabric();
        await using var wordSource = fabric.Create(new ValidSource(filePath));

        var wordBatch = await wordSource.GetNextBatch(CancellationToken.None);

        wordBatch.Should().HaveCount(4)
            .And.Contain(Word1)
            .And.Contain(Word2)
            .And.Contain(WordRuLocal1)
            .And.Contain(WordRuLocal2);
    }
    
    [Test]
    public async Task GetNextBatch_ReadSymbolLessWordLength_Return()
    {
        var filePath = await CreateFileAndGetPath(
            $"{Word1} {Word2}",
            $"{WordRuLocal1} {WordRuLocal2}");
        var fabric = CreateFabric(countSymbolRead:2);
        await using var wordSource = fabric.Create(new ValidSource(filePath));

        var wordBatch = await wordSource.GetNextBatch(CancellationToken.None);

        wordBatch.Should().HaveCount(4)
            .And.Contain(Word1)
            .And.Contain(Word2)
            .And.Contain(WordRuLocal1)
            .And.Contain(WordRuLocal2);
    }
    
    [Test]
    public async Task GetNextBatch_BatchIs2_Return2()
    {
        var filePath = await CreateFileAndGetPath(
            $"{Word1} {Word2}",
            $"{WordRuLocal1} {WordRuLocal2}");
        var fabric = CreateFabric(countSymbolRead:2, wordsInBatch:2);
        await using var wordSource = fabric.Create(new ValidSource(filePath));

        var wordBatch1 = await wordSource.GetNextBatch(CancellationToken.None);
        var wordBatch2 = await wordSource.GetNextBatch(CancellationToken.None);
        
        wordBatch1.Should().HaveCount(2)
            .And.Contain(Word1)
            .And.Contain(Word2);
        wordBatch2.Should().HaveCount(2)
            .And.Contain(WordRuLocal1)
            .And.Contain(WordRuLocal2);
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

    private FileWordSourceFabric CreateFabric(int wordsInBatch = 10, int countSymbolRead = 100)
    {
        return new FileWordSourceFabric(new FileWordSourceOption
        {
            WordsInBatch = wordsInBatch,
            CountSymbolRead = countSymbolRead
        });
    }
}