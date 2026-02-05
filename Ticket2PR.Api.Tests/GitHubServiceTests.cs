using Microsoft.Extensions.Logging;
using Moq;
using Ticket2PR.Api.Services;
using Xunit;

namespace Ticket2PR.Api.Tests;

public class GitHubServiceTests
{
    [Fact]
    public void ParseGitHubUrl_WithHttpsUrl_ReturnsParsedOwnerAndRepo()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GitHubService>>();
        var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        configMock.Setup(x => x["GitHub:Token"]).Returns("test_token");
        var service = new GitHubService(loggerMock.Object, configMock.Object);
        
        // Act
        var (owner, repo) = service.ParseGitHubUrl("https://github.com/testowner/testrepo");
        
        // Assert
        Assert.Equal("testowner", owner);
        Assert.Equal("testrepo", repo);
    }
    
    [Fact]
    public void ParseGitHubUrl_WithGitUrl_ReturnsParsedOwnerAndRepo()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GitHubService>>();
        var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        configMock.Setup(x => x["GitHub:Token"]).Returns("test_token");
        var service = new GitHubService(loggerMock.Object, configMock.Object);
        
        // Act
        var (owner, repo) = service.ParseGitHubUrl("https://github.com/testowner/testrepo.git");
        
        // Assert
        Assert.Equal("testowner", owner);
        Assert.Equal("testrepo", repo);
    }
    
    [Fact]
    public void ParseGitHubUrl_WithInvalidUrl_ThrowsArgumentException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GitHubService>>();
        var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        configMock.Setup(x => x["GitHub:Token"]).Returns("test_token");
        var service = new GitHubService(loggerMock.Object, configMock.Object);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.ParseGitHubUrl("https://github.com/invalid"));
    }
}
