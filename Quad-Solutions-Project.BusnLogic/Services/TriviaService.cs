using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quad_Solutions_Project.Models;
using Quad_Solutions_Project.Models.Interfaces;

namespace Quad_Solutions_Project.BusnLogic.Services
{
    public class TriviaService : ITriviaService
    {
        /// <summary>
        ///     Client used to retrieve questions from the Open Trivia Database.
        /// </summary>
        private readonly IOpenTdbClient _openTdbClient;

        /// <summary>
        ///     Server-side store where correct answers are kept until consumed.
        /// </summary>
        private readonly AnswerService _answerService;

        /// <summary>
        ///     Shared random instance used to shuffle answer options.
        /// </summary>
        private static readonly Random Rng = Random.Shared;

        /// <summary>
        /// Initialises a new instance of <see cref="TriviaService"/> with the
        /// required dependencies.
        /// </summary>
        /// <param name="openTdbClient">
        /// An implementation of <see cref="IOpenTdbClient"/> for fetching questions.
        /// </param>
        /// <param name="answerService">
        /// The singleton answer service where correct answers are persisted server-side.
        /// </param>
        public TriviaService(IOpenTdbClient openTdbClient, AnswerService answerService)
        {
            _openTdbClient = openTdbClient;
            _answerService = answerService;
        }

        /// <inheritdoc/>
        public async Task<List<TriviaQuestionResponse>> GetQuestionsAsync(
            GetQuestionsParameters parameters,
            CancellationToken cancellationToken = default)
        {
            List<OpenTdbQuestion> rawQuestions = await _openTdbClient
                .GetQuestionsAsync(parameters, cancellationToken)
                .ConfigureAwait(false);

            var clientSafeQuestions = new List<TriviaQuestionResponse>(rawQuestions.Count);

            foreach (OpenTdbQuestion raw in rawQuestions)
            {
                string questionId = Guid.NewGuid().ToString();

                // Store the correct answer server-side — it will never appear in the response.
                _answerService.Store(questionId, raw.CorrectAnswer);

                // Build the shuffled options list and map to the client-safe DTO.
                TriviaQuestionResponse mapped = MapToClientResponse(questionId, raw);
                clientSafeQuestions.Add(mapped);
            }

            return clientSafeQuestions;
        }

        /// <inheritdoc/>
        public CheckAnswerResponse? CheckAnswer(CheckAnswerRequest request)
        {
            // ConsumeAnswer atomically retrieves and removes the entry.
            // If the question ID is unknown or already consumed, it returns null.
            string? correctAnswer = _answerService.ConsumeAnswer(request.QuestionId);

            if (correctAnswer is null)
            {
                return null;
            }

            bool isCorrect = string.Equals(
                request.SelectedAnswer,
                correctAnswer,
                StringComparison.OrdinalIgnoreCase);

            return new CheckAnswerResponse
            {
                QuestionId = request.QuestionId,
                IsCorrect = isCorrect,
                CorrectAnswer = correctAnswer
            };
        }

        /// <summary>
        /// Maps a raw <see cref="OpenTdbQuestion"/> into a <see cref="TriviaQuestionResponse"/>
        /// that is safe to send to the client.  The correct answer is merged into
        /// the options list and the entire list is shuffled so the position of the
        /// correct answer cannot be inferred.
        /// </summary>
        /// <param name="questionId">
        /// The server-generated unique identifier to assign to this question.
        /// </param>
        /// <param name="raw">
        /// The raw question as returned by the Open Trivia Database.
        /// </param>
        /// <returns>
        /// A <see cref="TriviaQuestionResponse"/> with shuffled options and no
        /// indication of which option is correct.
        /// </returns>
        private static TriviaQuestionResponse MapToClientResponse(string questionId, OpenTdbQuestion raw)
        {
            // Combine correct answer with incorrect options, then shuffle.
            var allOptions = new List<string>(raw.IncorrectAnswers) { raw.CorrectAnswer };

            return new TriviaQuestionResponse
            {
                QuestionId = questionId,
                Category = raw.Category,
                Difficulty = ParseDifficulty(raw.Difficulty),
                Type = ParseType(raw.Type),
                Question = raw.Question,
                Options = allOptions
            };
        }

        /// <summary>
        /// Parses the difficulty string returned by the Open Trivia Database
        /// into the <see cref="QuestionDifficulty"/> enumeration.
        /// </summary>
        /// <param name="value">
        /// The lowercase difficulty string (e.g. "easy", "medium", "hard").
        /// </param>
        /// <returns>
        /// The corresponding <see cref="QuestionDifficulty"/> value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the value does not match any known difficulty level.
        /// </exception>
        private static QuestionDifficulty ParseDifficulty(string value)
        {
            return value.ToLowerInvariant() switch
            {
                "easy" => QuestionDifficulty.Easy,
                "medium" => QuestionDifficulty.Medium,
                "hard" => QuestionDifficulty.Hard,
                _ => throw new ArgumentException(
                                $"Unrecognised difficulty value: '{value}'.", nameof(value))
            };
        }

        /// <summary>
        /// Parses the type string returned by the Open Trivia Database
        /// into the <see cref="QuestionType"/> enumeration.
        /// </summary>
        /// <param name="value">
        /// The lowercase type string (e.g. "multiple", "boolean").
        /// </param>
        /// <returns>
        /// The corresponding <see cref="QuestionType"/> value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the value does not match any known question type.
        /// </exception>
        private static QuestionType ParseType(string value)
        {
            return value.ToLowerInvariant() switch
            {
                "multiple" => QuestionType.Multiple,
                "boolean" => QuestionType.Boolean,
                _ => throw new ArgumentException(
                                  $"Unrecognised question type value: '{value}'.", nameof(value))
            };
        }
    }

}
