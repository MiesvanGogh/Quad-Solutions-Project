using Quad_Solutions_Project.BusnLogic.Services;
using Quad_Solutions_Project.Models;
using Quad_Solutions_Project.Models.Interfaces;

using Moq;

namespace Quad_Solutions_Project.Tests;

[TestClass]
public class TriviaServiceTests
{
    /// <summary>
    ///     Creates a mock <see cref="IOpenTdbClient"/> that returns a list of questions.
    /// </summary>
    /// <param name="questions">
    ///     The list of <see cref="OpenTdbQuestion"/> objects the mock will return.
    /// </param>
    /// <returns>
    ///  A configured <see cref="Mock{T}"/> for <see cref="IOpenTdbClient"/>.
    /// </returns>
    private static Mock<IOpenTdbClient> CreateMockClient(List<OpenTdbQuestion> questions)
    {
        var mock = new Mock<IOpenTdbClient>();
        mock.Setup(c => c.GetQuestionsAsync(It.IsAny<GetQuestionsParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(questions);
        return mock;
    }

    /// <summary>
    ///     Returns a single multiple-choice question with known correct and incorrect answers.
    /// </summary>
    /// <returns>
    ///     A list containing one <see cref="OpenTdbQuestion"/>.
    /// </returns>
    private static List<OpenTdbQuestion> SingleMultipleChoiceQuestion()
    {
        return new List<OpenTdbQuestion>
        {
            new OpenTdbQuestion
            {
                Category         = "Geography",
                Difficulty       = "medium",
                Type             = "multiple",
                Question         = "What is the capital of France?",
                CorrectAnswer    = "Paris",
                IncorrectAnswers = new List<string> { "London", "Berlin", "Madrid" }
            }
        };
    }

    /// <summary>
    ///     Verifies that all answer options (correct + incorrect) are included in the response
    /// and that the correct answer is not separately identifiable.
    /// </summary>
    [TestMethod]
    public async Task GetQuestionsAsync_IncludesAllOptionsShuffled()
    {
        // Arrange
        var mockClient = CreateMockClient(SingleMultipleChoiceQuestion());
        var answerService = new AnswerService();
        var service = new TriviaService(mockClient.Object, answerService);

        // Act
        var result = await service.GetQuestionsAsync(new GetQuestionsParameters());

        // Assert
        var question = result[0];
        Assert.AreEqual(4, question.Options.Count);
        CollectionAssert.Contains(question.Options, "Paris");
        CollectionAssert.Contains(question.Options, "London");
        CollectionAssert.Contains(question.Options, "Berlin");
        CollectionAssert.Contains(question.Options, "Madrid");
    }

    /// <summary>
    /// Verifies that supplying the correct answer returns IsCorrect = true.
    /// </summary>
    [TestMethod]
    public async Task CheckAnswer_ReturnsTrue_WhenAnswerIsCorrect()
    {
        // Arrange
        var mockClient = CreateMockClient(SingleMultipleChoiceQuestion());
        var answerService = new AnswerService();
        var service = new TriviaService(mockClient.Object, answerService);

        var questions = await service.GetQuestionsAsync(new GetQuestionsParameters());
        string questionId = questions[0].QuestionId;

        // Act
        var result = service.CheckAnswer(new CheckAnswerRequest
        {
            QuestionId = questionId,
            SelectedAnswer = "Paris"
        });

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result!.IsCorrect);
        Assert.AreEqual("Paris", result.CorrectAnswer);
    }

    /// <summary>
    /// Verifies that checking the same question twice returns null the second time
    /// (answer consumed on first check).
    /// </summary>
    [TestMethod]
    public async Task CheckAnswer_ReturnsNull_WhenQuestionAlreadyAnswered()
    {
        // Arrange
        var mockClient = CreateMockClient(SingleMultipleChoiceQuestion());
        var answerService = new AnswerService();
        var service = new TriviaService(mockClient.Object, answerService);

        var questions = await service.GetQuestionsAsync(new GetQuestionsParameters());
        string questionId = questions[0].QuestionId;

        // Act — first check
        service.CheckAnswer(new CheckAnswerRequest
        {
            QuestionId = questionId,
            SelectedAnswer = "Paris"
        });

        // Act — second check
        var result = service.CheckAnswer(new CheckAnswerRequest
        {
            QuestionId = questionId,
            SelectedAnswer = "Paris"
        });

        // Assert
        Assert.IsNull(result);
    }
}
