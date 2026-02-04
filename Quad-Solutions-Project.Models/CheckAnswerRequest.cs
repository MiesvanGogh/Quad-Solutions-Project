using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quad_Solutions_Project.Models
{
    public class CheckAnswerRequest
    {
        /// <summary>
        ///     Gets or sets the unique identifier of the question being answered.
        /// </summary>
        public string QuestionId { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets answer option selected by the user. Must exactly match one of the
        /// </summary>
        public string SelectedAnswer { get; set; } = string.Empty;

    }
}
