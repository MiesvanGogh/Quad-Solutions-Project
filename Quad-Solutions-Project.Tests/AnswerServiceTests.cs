using Quad_Solutions_Project.BusnLogic.Services;

namespace Quad_Solutions_Project.Tests;

[TestClass]
public class AnswerServiceTests
{
    /// <summary>
    ///     Verifies that an answer that has been stored can be retrieved.
    /// </summary>
    [TestMethod]
    public void ConsumeAnswer_ReturnsCorrectAnswer_WhenQuestionExists()
    {
        // Arrange
        var store = new AnswerService();
        store.Store("q1", "Paris");

        // Act
        string? answer = store.ConsumeAnswer("q1");

        // Assert
        Assert.AreEqual("Paris", answer);
    }

    /// <summary>
    ///     Verifies that consuming an answer removes it from the store.
    /// </summary>
    [TestMethod]
    public void ConsumeAnswer_RemovesEntry_AfterFirstConsumption()
    {
        // Arrange
        var store = new AnswerService();
        store.Store("q1", "Paris");

        // Act — consume once
        store.ConsumeAnswer("q1");

        // Act — attempt to consume again
        string? secondAttempt = store.ConsumeAnswer("q1");

        // Assert
        Assert.IsNull(secondAttempt);
    }

    /// <summary>
    ///     Verifies that consuming a key that was never stored returns <c>null</c> without throwing.
    /// </summary>
    [TestMethod]
    public void ConsumeAnswer_ReturnsNull_WhenQuestionDoesNotExist()
    {
        // Arrange
        var store = new AnswerService();

        // Act
        string? answer = store.ConsumeAnswer("nonexistent-id");

        // Assert
        Assert.IsNull(answer);
    }

    /// <summary>
    ///     Verifies that <see cref="AnswerService.ConsumeAnswer"/> returns <c>null</c>.
    /// </summary>
    [TestMethod]
    public void ConsumeAnswer_ReturnsNull_WhenQuestionIdIsEmpty()
    {
        // Arrange
        var store = new AnswerService();

        // Act
        string? answer = store.ConsumeAnswer("");

        // Assert
        Assert.IsNull(answer);
    }

    /// <summary>
    ///     Verifies that <see cref="AnswerService.Store"/> throws an <see cref="ArgumentException"/> when the question ID is empty.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Store_ThrowsArgumentException_WhenQuestionIdIsEmpty()
    {
        // Arrange
        var store = new AnswerService();

        // Act & Assert
        store.Store("", "SomeAnswer");
    }

    /// <summary>
    ///     Verifies that <see cref="AnswerService.Store"/> throws an <see cref="ArgumentException"/> when the correct answer is empty.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Store_ThrowsArgumentException_WhenCorrectAnswerIsEmpty()
    {
        // Arrange
        var store = new AnswerService();

        // Act & Assert
        store.Store("q1", "");
    }

    /// <summary>
    ///     Verifies that <see cref="AnswerService.Store"/> throws an <see cref="ArgumentNullException"/> when the question ID is <c>null</c>.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Store_ThrowsArgumentNullException_WhenQuestionIdIsNull()
    {
        // Arrange
        var store = new AnswerService();

        // Act & Assert
        store.Store(null!, "SomeAnswer");
    }

    /// <summary>
    ///     Verifies that <see cref="AnswerService.Count"/> accurately reflects number of stored answers after additions and consumptions.
    /// </summary>
    [TestMethod]
    public void Count_ReflectsCurrentStoreSize()
    {
        // Arrange
        var store = new AnswerService();

        // Act & Assert
        Assert.AreEqual(0, store.Count);

        store.Store("q1", "A");
        store.Store("q2", "B");
        Assert.AreEqual(2, store.Count);

        store.ConsumeAnswer("q1");
        Assert.AreEqual(1, store.Count);
    }

    /// <summary>
    ///     Verifies that storing a key that already exists overwrites the previous value.
    /// </summary>
    [TestMethod]
    public void Store_OverwritesExistingEntry_WhenSameKeyIsUsed()
    {
        // Arrange  
        var store = new AnswerService();
        store.Store("q1", "OldAnswer");

        // Act
        store.Store("q1", "NewAnswer");

        // Assert
        string? answer = store.ConsumeAnswer("q1");
        Assert.AreEqual("NewAnswer", answer);
    }
}

