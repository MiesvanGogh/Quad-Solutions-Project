using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quad_Solutions_Project.Models
{
    public class TriviaQuestionResponse
    {
        /// <summary>
        ///     Gets or sets a unique identifier.
        /// </summary>
        public string QuestionId { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the category this question belongs to.
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the difficulty tier of the question.
        /// </summary>
        public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Medium;

        /// <summary>
        ///     Gets or sets the type of question (multiple-choice or boolean).
        /// </summary>
        public QuestionType Type { get; set; } = QuestionType.Multiple;

        /// <summary>
        ///     Gets or sets the question text, already decoded from HTML entities.
        /// </summary>
        public string Question { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets all answer options (correct + incorrect) in a randomly shuffled order.
        /// </summary>
        public List<string> Options { get; set; } = new();

    }
}
