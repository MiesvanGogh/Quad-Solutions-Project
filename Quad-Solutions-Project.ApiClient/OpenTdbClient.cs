using Quad_Solutions_Project.Models.Interfaces;
using Quad_Solutions_Project.Models;
using System.Text.Json;
using System.Web;

namespace Quad_Solutions_Project.ApiClient
{
    public class OpenTdbClient : IOpenTdbClient
    {
        /// <summary>
        /// The underlying HTTP client used for all outbound requests.
        /// Lifetime is managed by the <see cref="IHttpClientFactory"/> in the DI container.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// JSON deserialisation options shared across all API calls.
        /// </summary>
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Initialises a new instance of <see cref="OpenTdbClient"/> with the provided
        /// <see cref="HttpClient"/>.  The client should be registered as a typed client
        /// via <see cref="Microsoft.Extensions.Http.HttpClientFactoryServiceCollectionExtensions"/>.
        /// </summary>
        /// <param name="httpClient">
        /// The HTTP client instance, configured with the base URL of the Open Trivia Database.
        /// </param>
        public OpenTdbClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc/>
        public async Task<List<OpenTdbQuestion>> GetQuestionsAsync(
            GetQuestionsParameters parameters,
            CancellationToken cancellationToken = default)
        {
            string url = BuildRequestUrl(parameters);

            HttpResponseMessage httpResponse = await _httpClient
                .GetAsync(url, cancellationToken)
                .ConfigureAwait(false);

            httpResponse.EnsureSuccessStatusCode();

            OpenTdbApiResponse? apiResponse = await JsonSerializer
                .DeserializeAsync<OpenTdbApiResponse>(
                    httpResponse.Content.ReadAsStream(),
                    JsonOptions,
                    cancellationToken)
                .ConfigureAwait(false);

            if (apiResponse is null)
            {
                //TODO: Make custom exception class with a custom response code.
                throw new InvalidOperationException(
                    "The Open Trivia Database returned an empty or unparseable response body.");
            }

            return DecodeQuestions(apiResponse.Results);
        }

        /// <summary>
        /// Builds the query-string URL for the Open Trivia Database API from the
        /// supplied parameters.  Only non-null optional parameters are appended.
        /// </summary>
        /// <param name="parameters">
        /// The filter criteria to encode into the URL.
        /// </param>
        /// <returns>
        /// A relative URL path with query string, e.g.
        /// <c>api.php?amount=5&amp;category=9&amp;difficulty=medium&amp;type=multiple&amp;encode=url3986</c>.
        /// </returns>
        private static string BuildRequestUrl(GetQuestionsParameters parameters)
        {
            var queryParts = new List<string>
    {
        $"amount={parameters.Amount}",
        "encode=url3986"
    };

            if (parameters.CategoryId.HasValue)
            {
                queryParts.Add($"category={parameters.CategoryId.Value}");
            }

            if (parameters.Difficulty.HasValue)
            {
                queryParts.Add($"difficulty={parameters.Difficulty.Value.ToString().ToLowerInvariant()}");
            }

            if (parameters.Type.HasValue)
            {
                string typeValue = parameters.Type.Value switch
                {
                    QuestionType.Multiple => "multiple",
                    QuestionType.Boolean => "boolean",
                    _ => throw new ArgumentOutOfRangeException(
                        nameof(parameters),
                        parameters.Type.Value,
                        "Unsupported QuestionType value.")
                };

                queryParts.Add($"type={typeValue}");
            }

            return "api.php?" + string.Join("&", queryParts);
        }

        /// <summary>
        /// Decodes all HTML-entity-encoded text fields in a list of <see cref="OpenTdbQuestion"/>
        /// objects.  The Open Trivia Database encodes special characters (e.g. ampersands,
        /// apostrophes) as HTML entities; this method converts them back to plain text.
        /// </summary>
        /// <param name="questions">
        /// The raw question objects as deserialised from JSON.
        /// </param>
        /// <returns>
        /// A new list containing the same questions with all string fields decoded.
        /// </returns>
        private static List<OpenTdbQuestion> DecodeQuestions(List<OpenTdbQuestion> questions)
        {
            return questions.Select(DecodeQuestion).ToList();
        }

        /// <summary>
        /// Decodes all HTML-entity-encoded string fields on a single <see cref="OpenTdbQuestion"/>.
        /// </summary>
        /// <param name="question">
        /// The raw question object as deserialised from JSON.
        /// </param>
        /// <returns>
        /// A new <see cref="OpenTdbQuestion"/> with all string fields decoded.
        /// </returns>
        private static OpenTdbQuestion DecodeQuestion(OpenTdbQuestion question)
        {
            return new OpenTdbQuestion
            {
                Category = HttpUtility.HtmlDecode(question.Category),
                Difficulty = HttpUtility.HtmlDecode(question.Difficulty),
                Type = HttpUtility.HtmlDecode(question.Type),
                Question = HttpUtility.HtmlDecode(question.Question),
                CorrectAnswer = HttpUtility.HtmlDecode(question.CorrectAnswer),
                IncorrectAnswers = question.IncorrectAnswers
                                        .Select(HttpUtility.HtmlDecode)
                                        .ToList()
            };
        }
    }
}


