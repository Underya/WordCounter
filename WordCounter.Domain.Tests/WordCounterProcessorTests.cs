using Moq;
using WordCounter.Domain.SourceValidation;
using WordCounter.Domain.WordSource;

namespace WordCounter.Domain.Tests;

public class WordCounterProcessorTests
{
    private const string FileName = "FileName";
    private const string Word1 = "Word1";
    private const string Word2 = "Word2";

    private Mock<IWordSource> _wordSourceMock;
    private Mock<IWordSourceFabric> _wordSourceFabricMock;
    private Mock<IWordCountSaver> _wordCountSaverMock;
    private Mock<IWordValidator> _wordValidator;
    private Mock<ILogger> _loggerMock;

    [SetUp]
    public void InitialTest()
    {
        _wordSourceMock = new Mock<IWordSource>();
        _wordSourceFabricMock = new Mock<IWordSourceFabric>();
        _wordSourceFabricMock
            .Setup(m => m.Create(It.IsAny<ValidSource>()))
            .Returns(_wordSourceMock.Object);
        _wordCountSaverMock = new Mock<IWordCountSaver>();
        _wordValidator = new Mock<IWordValidator>();
        _loggerMock = new Mock<ILogger>();
    }

    [Test]
    public async Task Process_SaveOneName_MoqIsValid()
    {
        SetupWordSourceGetNextBatch(new[] { Word1 });
        var validSource = new ValidSource(FileName);
        var processor = CreateProcessor();

        await processor.Process(validSource, CancellationToken.None);

        _wordCountSaverMock.Verify(m => m.IncreaseWordCount(Word1, 1, It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task Process_SaveDifferentName_MoqIsValid()
    {
        SetupWordSourceGetNextBatch(new[] { Word1, Word2, Word1 });
        var validSource = new ValidSource(FileName);
        var processor = CreateProcessor();

        await processor.Process(validSource, CancellationToken.None);

        _wordCountSaverMock.Verify(m => m.IncreaseWordCount(Word1, 2, It.IsAny<CancellationToken>()));
        _wordCountSaverMock.Verify(m => m.IncreaseWordCount(Word2, 1, It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task Process_OneWordNotValid_NotSaveCount()
    {
        SetupWordSourceGetNextBatch(new[] { Word1, Word2, Word1 });
        var validSource = new ValidSource(FileName);
        var validationError = new ValidationError(FileName, "Word is not valid");
        var validationErrors = new[] { validationError };
        _wordValidator.Setup(mock => mock.ValidWord(Word1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationErrors);
        var processor = CreateProcessor();

        await processor.Process(validSource, CancellationToken.None);

        _wordCountSaverMock.Verify(m => m.IncreaseWordCount(Word2, 1, It.IsAny<CancellationToken>()));
        _wordCountSaverMock.Verify(m =>
            m.IncreaseWordCount(Word1, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _loggerMock.Verify(mock => mock.Log(validSource, validationErrors, It.IsAny<CancellationToken>()));
    }

    private void SetupWordSourceGetNextBatch(IEnumerable<string> words)
    {
        var sequence = new MockSequence();
        _wordSourceMock
            .InSequence(sequence)
            .Setup(m => m.GetNextBatch(It.IsAny<CancellationToken>()))
            .ReturnsAsync(words);
        _wordSourceMock.InSequence(sequence)
            .Setup(m => m.GetNextBatch(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
    }

    private WordCounterProcessor CreateProcessor()
    {
        return new WordCounterProcessor(
            new WordCounterProcessorOption
            {
                ThreadCount = 5
            },
            _wordSourceFabricMock.Object,
            _wordCountSaverMock.Object,
            _wordValidator.Object,
            _loggerMock.Object);
    }
}