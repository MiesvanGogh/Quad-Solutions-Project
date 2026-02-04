using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quad_Solutions_Project.Models
{
    public class GetQuestionsParameters
    {
        /// <summary>
        ///     Gets or sets the number of questions to retrieve. Must be between 1 and 50 (API limit).
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        ///     Gets or sets the numeric category ID as defined by the Open Trivia Database.
        /// </summary>
        public int? CategoryId { get; set; } = null;

        /// <summary>
        ///     Gets or sets the difficulty filter to apply.
        /// </summary>
        public QuestionDifficulty? Difficulty { get; set; } = null;

        /// <summary>
        ///     Gets or sets the question-type filter to apply.
        /// </summary>
        public QuestionType? Type { get; set; } = null;
    }
}
