using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quad_Solutions_Project.BusnLogic.Services
{
    /// <summary>
    ///     Stores and manages correct answers keyed by question ID.
    /// </summary>
    public class AnswerService
    {
        /// <summary>
        ///     The underlying dictionary that maps question identifiers to their correct answers.
        /// </summary>
        private readonly ConcurrentDictionary<string, string> _answers = new();

        /// <summary>
        ///     Stores the correct answer for a question.
        /// </summary>
        /// <param name="questionId">
        ///     The unique question identifier.
        /// </param>
        /// <param name="correctAnswer">
        ///     The correct answer text.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="questionId"/> or <paramref name="correctAnswer"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="questionId"/> or <paramref name="correctAnswer"/> is empty or whitespace.
        /// </exception>
        public void Store(string questionId, string correctAnswer)
        {
            ArgumentNullException.ThrowIfNull(questionId);
            ArgumentNullException.ThrowIfNull(correctAnswer);

            if (string.IsNullOrWhiteSpace(questionId))
            {
                throw new ArgumentException("Question ID must not be empty or whitespace.", nameof(questionId));
            }

            if (string.IsNullOrWhiteSpace(correctAnswer))
            {
                throw new ArgumentException("Correct answer must not be empty or whitespace.", nameof(correctAnswer));
            }

            _answers[questionId] = correctAnswer;
        }

        /// <summary>
        ///     Automically retrieves and removes a stored answer.
        /// </summary>
        /// <param name="questionId">
        ///     The question identifier.
        /// </param>
        /// <returns>
        ///     The correct answer, or <c>null</c> if not found.
        /// </returns>
        public string? ConsumeAnswer(string questionId)
        {
            if (string.IsNullOrWhiteSpace(questionId))
            {
                return null;
            }

            return _answers.TryRemove(questionId, out string? answer) ? answer : null;
        }

        /// <summary>
        ///     Gets the number of stored answers.
        /// </summary>
        public int Count
        {
            get { return _answers.Count; }
        }
    }
}

