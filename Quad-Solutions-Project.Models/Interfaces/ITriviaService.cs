using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quad_Solutions_Project.Models.Interfaces
{
    public interface ITriviaService
    {
        /// <summary>
        ///	    Fetches trivia questions and returns safe questions without exposing the correct answers.
        /// </summary>
        /// <param name="parameters">
        ///	    Options that control the number of questions and applied filters.
        /// </param>
        /// <param name="cancellationToken">
        ///	    The cancellation token to cancel the operation.  Optional.
        /// </param>
        /// <returns>
        ///	    A list of <see cref="TriviaQuestionResponse"/> objects.
        /// </returns>
        Task<List<TriviaQuestionResponse>> GetQuestionsAsync(
            GetQuestionsParameters parameters,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///	    Checks a user's answer against the stored correct answer.
        /// </summary>
        /// <param name="request">
        ///	    The question ID and the user's selected answer.
        /// </param>
        /// <returns>
        ///	    A <see cref="CheckAnswerResponse"/> with the result.
        /// </returns>
        CheckAnswerResponse? CheckAnswer(CheckAnswerRequest request);

    }
}
