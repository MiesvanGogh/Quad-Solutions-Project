namespace Quad_Solutions_Project.Models
{
    public class CheckAnswerResponse
    {
        /// <summary>
        ///     Gets or sets the unique identifier of the question that was evaluated.
        /// </summary>
        public string QuestionId { get; set; } = string.Empty;

        /// <summary>
        ///     <c>true</c> if the user's selected answer matches the correct answer;
        ///     <c>false</c> otherwise.
        /// </summary>
        public bool IsCorrect { get; set; } = false;

        /// <summary>
        ///     Gets or sets the correct answer for this question.
        /// </summary>
        public string CorrectAnswer { get; set; } = string.Empty;

    }
}
