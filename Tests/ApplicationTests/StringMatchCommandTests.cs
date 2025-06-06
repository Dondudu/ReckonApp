// Tests/UnitTests/Application/Commands/StringMatchCommandHandlerTests.cs
using Microsoft.Extensions.Options;
using Moq;
using ReckonApp.Application.StringSearch.Commands;
using ReckonApp.Application.StringSearch.Mappings;
using ReckonApp.Domain.Models;
using ReckonApp.Domain.Services.StringMatcher;
using ReckonApp.Infrastructure.ExternalApi;
using ReckonApp.Infrastructure.ExternalApi.Models;
using ReckonApp.Models;

namespace Tests.ApplicationTests
{
    public class StringMatchCommandHandlerTests
    {
        private readonly Mock<IReckonApiClient> _reckonApiClientMock;
        private readonly Mock<IStringMatcher> _stringMatcherMock;
        private readonly Mock<IStringMatchResultMapper> _mapperMock;
        private readonly Mock<IOptions<ApplicationSettings>> _optionsMock;
        private readonly ApplicationSettings _settings;
        private readonly StringMatchCommandHandler _handler;
        private readonly CancellationToken _cancellationToken;

        public StringMatchCommandHandlerTests()
        {
            _reckonApiClientMock = new Mock<IReckonApiClient>();
            _stringMatcherMock = new Mock<IStringMatcher>();
            _mapperMock = new Mock<IStringMatchResultMapper>();
            _settings = new ApplicationSettings { CandidateName = "John Doe" };
            _optionsMock = new Mock<IOptions<ApplicationSettings>>();
            _optionsMock.Setup(o => o.Value).Returns(_settings);
            _handler = new StringMatchCommandHandler(
                _reckonApiClientMock.Object,
                _mapperMock.Object,
                _stringMatcherMock.Object,
                _optionsMock.Object);
            _cancellationToken = CancellationToken.None;
        }

        [Fact]
        public async Task Handle_ValidInput_ReturnsExpectedResult()
        {
            // Arrange
            var command = new StringMatchCommand();
            var textResult = new StringToSearchResult { Text = "Hello World" };
            var subTextsResult = new SubTextsResult { SubTexts = new List<string> { "Hello", "World" } };
            var matchResult = new StringMatchResult
            {
                SubStringIndices = new Dictionary<string, List<int>>
                {
                    { "Hello", new List<int> { 0 } },
                    { "World", new List<int> { 6 } }
                }
            };
            var submitResults = new SubmitResultsModel
            {
                Candidate = "John Doe",
                Text = "Hello World",
                Results = new List<SubmitResultsItem>
                {
                    new SubmitResultsItem("Hello", "0"),
                    new SubmitResultsItem("World", "6")
                }
            };

            _reckonApiClientMock.Setup(c => c.GetTextToSearchAsync())
                .ReturnsAsync(textResult);
            _reckonApiClientMock.Setup(c => c.GetSubTextsAsync())
                .ReturnsAsync(subTextsResult);
            _stringMatcherMock.Setup(m => m.MatchStrings("Hello World", subTextsResult.SubTexts))
                .Returns(matchResult);
            _mapperMock.Setup(m => m.Map(matchResult, "Hello World", "John Doe"))
                .Returns(submitResults);
            _reckonApiClientMock.Setup(c => c.PostSubmitResultsAsync(submitResults))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.Candidate);
            Assert.Equal("Hello World", result.Text);
            Assert.Equal(2, result.Results.Count);
            Assert.Contains(result.Results, r => r.Subtext == "Hello" && r.Result == "0");
            Assert.Contains(result.Results, r => r.Subtext == "World" && r.Result == "6");

            _reckonApiClientMock.Verify(c => c.GetTextToSearchAsync(), Times.Once());
            _reckonApiClientMock.Verify(c => c.GetSubTextsAsync(), Times.Once());
            _stringMatcherMock.Verify(m => m.MatchStrings("Hello World", subTextsResult.SubTexts), Times.Once());
            _mapperMock.Verify(m => m.Map(matchResult, "Hello World", "John Doe"), Times.Once());
            _reckonApiClientMock.Verify(c => c.PostSubmitResultsAsync(submitResults), Times.Once());
        }

        [Fact]
        public async Task Handle_EmptySubTexts_ReturnsEmptyResult()
        {
            // Arrange
            var command = new StringMatchCommand();
            var textResult = new StringToSearchResult { Text = "Hello World" };
            var subTextsResult = new SubTextsResult { SubTexts = new List<string>() };
            var matchResult = new StringMatchResult
            {
                SubStringIndices = new Dictionary<string, List<int>>()
            };
            var submitResults = new SubmitResultsModel
            {
                Candidate = "John Doe",
                Text = "Hello World",
                Results = new List<SubmitResultsItem>()
            };

            _reckonApiClientMock.Setup(c => c.GetTextToSearchAsync())
                .ReturnsAsync(textResult);
            _reckonApiClientMock.Setup(c => c.GetSubTextsAsync())
                .ReturnsAsync(subTextsResult);
            _stringMatcherMock.Setup(m => m.MatchStrings("Hello World", subTextsResult.SubTexts))
                .Returns(matchResult);
            _mapperMock.Setup(m => m.Map(matchResult, "Hello World", "John Doe"))
                .Returns(submitResults);
            _reckonApiClientMock.Setup(c => c.PostSubmitResultsAsync(submitResults))
                .ReturnsAsync(true); // Fixed: Returns Task<bool> with true

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.Candidate);
            Assert.Equal("Hello World", result.Text);
            Assert.Empty(result.Results);

            _stringMatcherMock.Verify(m => m.MatchStrings("Hello World", subTextsResult.SubTexts), Times.Once());
            _mapperMock.Verify(m => m.Map(matchResult, "Hello World", "John Doe"), Times.Once());
            _reckonApiClientMock.Verify(c => c.PostSubmitResultsAsync(submitResults), Times.Once());
        }

        [Fact]
        public async Task Handle_PostSubmitResultsFails_ThrowsException()
        {
            // Arrange
            var command = new StringMatchCommand();
            var textResult = new StringToSearchResult { Text = "Hello World" };
            var subTextsResult = new SubTextsResult { SubTexts = new List<string> { "Hello" } };
            var matchResult = new StringMatchResult
            {
                SubStringIndices = new Dictionary<string, List<int>>
                {
                    { "Hello", new List<int> { 0 } }
                }
            };
            var submitResults = new SubmitResultsModel
            {
                Candidate = "John Doe",
                Text = "Hello World",
                Results = new List<SubmitResultsItem>
                {
                    new SubmitResultsItem("Hello", "0")
                }
            };

            _reckonApiClientMock.Setup(c => c.GetTextToSearchAsync())
                .ReturnsAsync(textResult);
            _reckonApiClientMock.Setup(c => c.GetSubTextsAsync())
                .ReturnsAsync(subTextsResult);
            _stringMatcherMock.Setup(m => m.MatchStrings("Hello World", subTextsResult.SubTexts))
                .Returns(matchResult);
            _mapperMock.Setup(m => m.Map(matchResult, "Hello World", "John Doe"))
                .Returns(submitResults);
            _reckonApiClientMock.Setup(c => c.PostSubmitResultsAsync(submitResults))
                .ReturnsAsync(false);


            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, _cancellationToken));
        }
    }
}