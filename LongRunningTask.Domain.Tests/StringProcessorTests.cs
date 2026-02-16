namespace LongRunningTask.Domain.Tests
{
    public class StringProcessorTests
    {
        StringProcessor _stringProcessor;

        [SetUp]
        public void Setup()
        {
            _stringProcessor = new StringProcessor();
        }

        [Test]
        public void EmptyStringsShouldFail()
        {
            //Arrange
            string input = string.Empty;
            // Act & Assert
            Assert.Catch<ArgumentException>(() => _stringProcessor.Process(input));
        }

        [Test]
        public void BasicExampleShouldPass()
        {
            // Arrange
            string input = "Hello, World!";

            // Act
            var processedString = _stringProcessor.Process(input);

            // Assert
            Assert.That(processedString, Is.EqualTo(" 1!1,1H1W1d1e1l3o2r1/SGVsbG8sIFdvcmxkIQ=="));
        }
    }
}