using LondonStyle.Core.Interfaces;
using LondonStyle.Core.Models;
using LondonStyle.Infrastructure.Services;
using Moq;

namespace LondonStyle.Tests;

public class TextAnalysisServiceTests
{
    [Fact]
    public async Task AnalyzeTextAsync_WithSimpleText_ReturnsAnalysisWithGradeLevel()
    {
        // Arrange
        var service = new TextAnalysisService();
        var request = new TextAnalysisRequest
        {
            Text = "The cat sat on the mat.",
            TargetGradeLevel = 7
        };

        // Act
        var result = await service.AnalyzeTextAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Text, result.OriginalText);
        Assert.True(result.OriginalGradeLevel >= 0);
        Assert.False(result.RevisedByAI);
    }

    [Fact]
    public async Task AnalyzeTextAsync_DetectsPassiveVoice()
    {
        // Arrange
        var service = new TextAnalysisService();
        var request = new TextAnalysisRequest
        {
            Text = "The document was reviewed by the committee.",
            TargetGradeLevel = 7
        };

        // Act
        var result = await service.AnalyzeTextAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result.Issues, i => i.Type == "PassiveVoice");
    }

    [Fact]
    public async Task AnalyzeTextAsync_DetectsLongSentences()
    {
        // Arrange
        var service = new TextAnalysisService();
        var longSentence = "This is a very long sentence that contains more than twenty words " +
                          "and should be flagged as hard to read because it goes on and on without stopping.";
        var request = new TextAnalysisRequest
        {
            Text = longSentence,
            TargetGradeLevel = 7
        };

        // Act
        var result = await service.AnalyzeTextAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result.Issues, i => i.Type == "HardToRead");
    }

    [Fact]
    public async Task SimplifySentenceAsync_WithoutLlmProvider_ReturnsOriginalSentence()
    {
        // Arrange
        var service = new TextAnalysisService();
        var sentence = "The aforementioned documentation was thoroughly examined.";

        // Act
        var result = await service.SimplifySentenceAsync(sentence, 7);

        // Assert
        Assert.Equal(sentence, result);
    }

    [Fact]
    public async Task AnalyzeTextAsync_WithLlmProvider_CallsProviderForSimplification()
    {
        // Arrange
        var mockLlmProvider = new Mock<ILlmProvider>();
        mockLlmProvider
            .Setup(p => p.GenerateResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("The committee reviewed the document.");

        var service = new TextAnalysisService(mockLlmProvider.Object);
        var request = new TextAnalysisRequest
        {
            Text = "The extremely comprehensive and detailed documentation was meticulously and thoroughly reviewed by the oversight committee.",
            TargetGradeLevel = 7
        };

        // Act
        var result = await service.AnalyzeTextAsync(request);

        // Assert
        Assert.NotNull(result);
        if (result.OriginalGradeLevel > request.TargetGradeLevel)
        {
            mockLlmProvider.Verify(
                p => p.GenerateResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
