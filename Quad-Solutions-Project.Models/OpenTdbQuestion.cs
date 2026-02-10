using System.Text.Json.Serialization;

namespace Quad_Solutions_Project.Models
{
    public class OpenTdbQuestion
    {
        /// <summary>
        ///     Gets or sets the category this question belongs to.
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the difficulty of the question.
        /// </summary>
        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the question type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the question text.
        /// </summary>
        [JsonPropertyName("question")]
        public string Question { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the single correct answer for this question.
        /// </summary>
        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the list of incorrect answer options.
        /// </summary>
        [JsonPropertyName("incorrect_answers")]
        public List<string> IncorrectAnswers { get; set; } = new();
    }
}
