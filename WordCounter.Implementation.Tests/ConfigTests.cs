namespace WordCounter.Implementation.Tests;

[SetUpFixture]
public class ConfigTests
{
    [OneTimeSetUp]
    public void InitializeAllTests()
    {
        TestsFileHelper.CreateDirectory();
    }
}