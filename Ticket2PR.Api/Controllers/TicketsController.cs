using Microsoft.AspNetCore.Mvc;
using Ticket2PR.Api.Models;
using Ticket2PR.Api.Services;

namespace Ticket2PR.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ILogger<TicketsController> _logger;
    private readonly GitService _gitService;
    private readonly GitHubService _githubService;
    private readonly CodeGenerationService _codeGenerationService;

    public TicketsController(
        ILogger<TicketsController> logger,
        GitService gitService,
        GitHubService githubService,
        CodeGenerationService codeGenerationService)
    {
        _logger = logger;
        _gitService = gitService;
        _githubService = githubService;
        _codeGenerationService = codeGenerationService;
    }

    [HttpPost]
    public async Task<ActionResult<TicketResponse>> ProcessTicket([FromBody] TicketRequest request)
    {
        try
        {
            _logger.LogInformation("Processing ticket for repository: {GitHubUrl}", request.GitHubUrl);

            // Create a temporary directory for the cloned repository
            var tempPath = Path.Combine(Path.GetTempPath(), $"ticket2pr-{Guid.NewGuid()}");
            
            try
            {
                // 1. Clone the repository
                _gitService.CloneRepository(request.GitHubUrl, tempPath);

                // 2. Create and checkout a feature branch
                var branchName = _gitService.CreateAndCheckoutBranch(tempPath, request.TicketDescription);

                // 3. Implement code changes
                _codeGenerationService.ImplementChanges(tempPath, request.TicketDescription);

                // 4. Commit changes
                _gitService.CommitChanges(tempPath, $"Implement: {request.TicketDescription}");

                // 5. Push branch to GitHub
                var githubToken = _githubService.GetGitHubToken();
                _gitService.PushBranch(tempPath, branchName, githubToken);

                // 6. Create pull request
                var prUrl = await _githubService.CreatePullRequest(
                    request.GitHubUrl,
                    branchName,
                    request.TicketDescription);

                return Ok(new TicketResponse
                {
                    BranchName = branchName,
                    PullRequestUrl = prUrl,
                    Status = "Success",
                    Message = "Pull request created successfully"
                });
            }
            finally
            {
                // Clean up temporary directory
                if (Directory.Exists(tempPath))
                {
                    try
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to clean up temporary directory: {TempPath}", tempPath);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing ticket");
            return StatusCode(500, new TicketResponse
            {
                BranchName = string.Empty,
                PullRequestUrl = string.Empty,
                Status = "Error",
                Message = ex.Message
            });
        }
    }
}
