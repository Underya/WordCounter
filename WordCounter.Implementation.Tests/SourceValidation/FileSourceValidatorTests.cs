using FluentAssertions;
using WordCounter.Implementation.SourceValidation;

namespace WordCounter.Implementation.Tests.SourceValidation;

public class FileSourceValidatorTests
{
    private const string FileName = $"{nameof(FileSourceValidatorTests)}.txt";

    [TearDown]
    public void Clear()
    {
        TestsFileHelper.DeleteIfExists(FileName);
    }
    
    [Test]
    public async Task ValidationFile_FileExists_ReturnValidFile()
    {
        var filePath = TestsFileHelper.CreateFileAndGetPath(FileName);
        var validator = new FileSourceValidator();

        var (validationFile, errors) = await validator.ValidationFile(filePath, CancellationToken.None);

        errors.Should().BeEmpty();
        validationFile.Should().NotBeNull();
        validationFile!.FileName.Should().Contain(FileName);
    }

    [Test]
    public async Task ValidationFile_FileNotExists_ReturnValidationError()
    {
        var filePath = TestsFileHelper.GetFilePath(FileName);
        var validator = new FileSourceValidator();

        var (validationFile, errors) = await validator.ValidationFile(filePath, CancellationToken.None);

        validationFile.Should().BeNull();
        errors.Should().ContainSingle()
            .And.OnlyContain(error => error.FileName.Contains(FileName) 
                                      && error.ValidationMessage.Contains("file not exists"));
    }

}