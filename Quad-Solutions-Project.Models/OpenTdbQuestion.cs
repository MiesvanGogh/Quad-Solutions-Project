using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quad_Solutions_Project.Models
{
    public class OpenTdbQuestion
    {
        /// <summary>
        /// Gets or sets the category this question belongs to.
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the difficulty of the question.
        /// </summary>
        public string Difficulty { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the question type.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        ///     Get or sets the question text.
        /// </summary>
        public string Question { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets single correct answer for this question.
        /// </summary>
        public string CorrectAnswer { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets list of incorrect answer options, HTML-entity-encoded.
        /// </summary>
        public List<string> IncorrectAnswers { get; set; } = new();
    }
}
