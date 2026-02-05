using Microsoft.Extensions.Logging;
using Moq;
using Ticket2PR.Api.Services;
using Xunit;

namespace Ticket2PR.Api.Tests;

public class CodeGenerationServiceTests
{
    [Fact]
    public void ImplementChanges_CreatesImplementationFile()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CodeGenerationService>>();
        var service = new CodeGenerationService(loggerMock.Object);
        var tempPath = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}");
        Directory.CreateDirectory(tempPath);
        
        try
        {
            // Act
            service.ImplementChanges(tempPath, "Test ticket description");
            
            // Assert
            var implementationFile = Path.Combine(tempPath, "IMPLEMENTATION.md");
            Assert.True(File.Exists(implementationFile));
            
            var content = File.ReadAllText(implementationFile);
            Assert.Contains("Test ticket description", content);
            Assert.Contains("Implementation for Ticket", content);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }
}
