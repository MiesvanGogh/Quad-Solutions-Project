using Quad_Solutions_Project.Models.Interfaces;
using Quad_Solutions_Project.Models;
using System.Text.Json;
using System.Web;

namespace Quad_Solutions_Project.ApiClient
{
    public class OpenTdbClient : IOpenTdbClient
    {
        /// <summary>
        ///     The underlying HTTP client used for all outbound requests.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        ///     JSON deserialisation options shared across all API calls.
        /// </summary>
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        ///     Initialises a new instance of <see cref="OpenTdbClient"/>.
        /// </summary>
        /// <param name="httpClient">
        ///     The HTTP client instance.
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
        ///     Builds the query-string URL for the Open Trivia Database API from the supplied parameters.
        /// </summary>
        /// <param name="parameters">
        ///     The filter criteria to encode into the URL.
        /// </param>
        /// <returns>
        ///     A relative URL path with query string.
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
        ///     Decodes all HTML-entity-encoded text fields in a list of <see cref="OpenTdbQuestion"/> objects.
        /// </summary>
        /// <param name="questions">
        ///     The raw question objects as deserialised from JSON.
        /// </param>
        /// <returns>
        ///     A new list containing the same questions with all string fields decoded.
        /// </returns>
        private static List<OpenTdbQuestion> DecodeQuestions(List<OpenTdbQuestion> questions)
        {
            return questions.Select(DecodeQuestion).ToList();
        }

        /// <summary>
        ///     Decodes all HTML-entity-encoded string fields on a single <see cref="OpenTdbQuestion"/>.
        /// </summary>
        /// <param name="question">
        ///     The raw question object as deserialised from JSON.
        /// </param>
        /// <returns>
        ///     A new <see cref="OpenTdbQuestion"/> with all string fields decoded.
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


