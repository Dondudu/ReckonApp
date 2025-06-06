using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using ReckonApp.Infrastructure.ExternalApi;
using ReckonApp.Infrastructure.ExternalApi.Models;
using ReckonApp.Models;
using System.Net;
using System.Text.Json;

namespace ReckonApp.Infrastructure.Tests
{
    public class ReckonApiClientTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IOptions<ReckonApiSettings>> _settingsMock;
        private readonly ReckonApiSettings _reckonApiSettings;
        private readonly ReckonApiClient _reckonApiClient;

        public ReckonApiClientTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _settingsMock = new Mock<IOptions<ReckonApiSettings>>();
            _reckonApiSettings = new ReckonApiSettings
            {
                SubTextsUrl = "https://testreckon.com/subtexts",
                TextToSearchUrl = "https://testreckon.com/texttosearch",
                SubmitResultsUrl = "https://testreckon.com/submitresults"
            };
            _settingsMock.Setup(s => s.Value).Returns(_reckonApiSettings);
            _reckonApiClient = new ReckonApiClient(_httpClient, _settingsMock.Object);
        }

        [Fact]
        public async Task GetSubTextsAsync_SuccessfulResponse_ReturnsSubTextsModel()
        {
            // Arrange
            var subTextsModel = new SubTextsResult { SubTexts = new List<string> { "text1", "text2" } };
            var jsonResponse = JsonSerializer.Serialize(subTextsModel);
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act
            var result = await _reckonApiClient.GetSubTextsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(subTextsModel.SubTexts, result.SubTexts);
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == _reckonApiSettings.SubTextsUrl),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetSubTextsAsync_InvalidJson_ThrowsException()
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("invalid json")
                });

            await Assert.ThrowsAsync<JsonException>(() => _reckonApiClient.GetSubTextsAsync());
        }

        [Fact]
        public async Task GetTextToSearchAsync_SuccessfulResponse_ReturnsStringToSearchModel()
        {
            // Arrange
            var stringToSearchModel = new StringToSearchResult { Text = "sample text" };
            var jsonResponse = JsonSerializer.Serialize(stringToSearchModel);
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act
            var result = await _reckonApiClient.GetTextToSearchAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(stringToSearchModel.Text, result.Text);
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == _reckonApiSettings.TextToSearchUrl),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task PostSubmitResultsAsync_SuccessfulResponse_ReturnsTrue()
        {
            // Arrange
            var submitResultsModel = new SubmitResultsModel() { Candidate = "Test", Results = new List<SubmitResultsItem>(), Text = "ABC" };
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            var result = await _reckonApiClient.PostSubmitResultsAsync(submitResultsModel);

            // Assert
            Assert.True(result);
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri.ToString() == _reckonApiSettings.SubmitResultsUrl),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}