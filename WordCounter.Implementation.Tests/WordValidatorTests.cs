using FluentAssertions;
using WordCounter.Domain.SourceValidation;

namespace WordCounter.Implementation.Tests;

public class WordValidatorTests
{
    private ValidSource Source => new ValidSource("fileName");
    
    [TestCase("word")]
    [TestCase("words")]
    [TestCase("длинноеслово")]
    [TestCase("abcdeabcdeabcdeabcde")] // 20 символов
    [TestCase("абвгдабвгдабвгдабвгд")] // 20 символов
    public async Task ValidateWord_WordIsValid_ReturnEmpty(string word)
    {
        var validator = new WordValidator();

        var result = await validator.ValidateWord(Source, word, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [TestCase("a")]
    [TestCase("ab")]
    public async Task ValidateWord_WordIsShort_ReturnError(string word)
    {
        var validator = new WordValidator();

        var result = await validator.ValidateWord(Source, word, CancellationToken.None);

        result.Should().ContainSingle()
            .And.Contain(erorr => erorr.ValidationMessage.Contains("3"));
    }
    
    [TestCase("abcdeabcdeabcdeabcdea")]  // 21 символ
    [TestCase("abcdeabcdeabcdeabcdeaa")] // 22 символа
    [TestCase("абвгдабвгдабвгдабвгда")]  // 21 символ
    [TestCase("абвгдабвгдабвгдабвгдаб")] // 22 символа
    public async Task ValidateWord_WordIsLong_ReturnError(string word)
    {
        var validator = new WordValidator();

        var result = await validator.ValidateWord(Source, word, CancellationToken.None);

        result.Should().ContainSingle()
            .And.Contain(erorr => erorr.ValidationMessage.Contains("20"));
    }
}