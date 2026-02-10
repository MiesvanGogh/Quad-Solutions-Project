using Quad_Solutions_Project.Models;
using Quad_Solutions_Project.ApiClient;
using System.Text;
using System.Text.Json;

namespace Quad_Solutions_Project.Tests;

[TestClass]
public class OpenTdbClientTests
{
    /// <summary>
    /// A minimal <see cref="HttpMessageHandler"/> that returns a fixed
    /// <see cref="HttpResponseMessage"/> for every request.
    /// </summary>
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        /// <summary>
        /// The response that will be returned for every request.
        /// </summary>
        public HttpResponseMessage ResponseToReturn { get; set; } = null!;

        /// <inheritdoc/>
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(ResponseToReturn);
        }
    }

    /// <summary>
    /// Serialises an <see cref="OpenTdbApiResponse"/> to a JSON byte array
    /// and wraps it in an <see cref="HttpResponseMessage"/> with status 200.
    /// </summary>
    /// <param name="apiResponse">
    /// The API response object to serialise.
    /// </param>
    /// <returns>
    /// An <see cref="HttpResponseMessage"/> whose body contains the serialised JSON.
    /// </returns>
    private static HttpResponseMessage CreateJsonResponse(OpenTdbApiResponse apiResponse)
    {
        string json = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(Encoding.UTF8.GetBytes(json))
            {
                Headers = { { "Content-Type", "application/json" } }
            }
        };
    }

    /// <summary>
    /// Creates an <see cref="OpenTdbClient"/> wired to the provided mock handler.
    /// </summary>
    /// <param name="handler">
    /// The mock handler that will intercept all requests.
    /// </param>
    /// <returns>
    /// A configured <see cref="OpenTdbClient"/> instance.
    /// </returns>
    private static OpenTdbClient CreateClient(MockHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://opentdb.com/")
        };

        return new OpenTdbClient(httpClient);
    }

    /// <summary>
    /// Verifies that a successful API response with questions is correctly
    /// deserialised and returned.
    /// </summary>
    [TestMethod]
    public async Task GetQuestionsAsync_ReturnsQuestions_OnSuccessResponse()
    {
        // Arrange
        var handler = new MockHttpMessageHandler
        {
            ResponseToReturn = CreateJsonResponse(new OpenTdbApiResponse
            {
                ResponseCode = 0,
                Results = new List<OpenTdbQuestion>
                {
                    new OpenTdbQuestion
                    {
                        Category         = "Geography",
                        Difficulty       = "easy",
                        Type             = "multiple",
                        Question         = "What is 2+2?",
                        CorrectAnswer    = "4",
                        IncorrectAnswers = new List<string> { "3", "5", "6" }
                    }
                }
            })
        };

        var client = CreateClient(handler);

        // Act
        var result = await client.GetQuestionsAsync(new GetQuestionsParameters { Amount = 1 });

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Geography", result[0].Category);
        Assert.AreEqual("4", result[0].CorrectAnswer);
    }

    /// <summary>
    /// Verifies that HTML entities are decoded into plain text.
    /// </summary>
    [TestMethod]
    public async Task GetQuestionsAsync_DecodesHtmlEntities()
    {
        // Arrange
        var handler = new MockHttpMessageHandler
        {
            ResponseToReturn = CreateJsonResponse(new OpenTdbApiResponse
            {
                ResponseCode = 0,
                Results = new List<OpenTdbQuestion>
                {
                    new OpenTdbQuestion
                    {
                        Category         = "Science &amp; Nature",
                        Difficulty       = "medium",
                        Type             = "multiple",
                        Question         = "What is &pi;?",
                        CorrectAnswer    = "3.14",
                        IncorrectAnswers = new List<string> { "2.71" }
                    }
                }
            })
        };

        var client = CreateClient(handler);

        // Act
        var result = await client.GetQuestionsAsync(new GetQuestionsParameters { Amount = 1 });

        // Assert
        Assert.AreEqual("Science & Nature", result[0].Category);
        Assert.AreEqual("What is π?", result[0].Question);
    }
}

