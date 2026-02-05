using LibGit2Sharp;

namespace Ticket2PR.Api.Services;

public class GitService
{
    private readonly ILogger<GitService> _logger;

    public GitService(ILogger<GitService> logger)
    {
        _logger = logger;
    }

    public string CloneRepository(string repoUrl, string localPath)
    {
        _logger.LogInformation("Cloning repository from {RepoUrl} to {LocalPath}", repoUrl, localPath);
        
        if (Directory.Exists(localPath))
        {
            Directory.Delete(localPath, true);
        }

        Repository.Clone(repoUrl, localPath);
        return localPath;
    }

    public string CreateAndCheckoutBranch(string repoPath, string ticketDescription)
    {
        _logger.LogInformation("Creating and checking out branch in {RepoPath}", repoPath);
        
        using var repo = new Repository(repoPath);
        
        // Generate a unique branch name (max 15 characters)
        var branchName = GenerateBranchName(ticketDescription);
        
        // Create and checkout the branch
        var branch = repo.CreateBranch(branchName);
        Commands.Checkout(repo, branch);
        
        _logger.LogInformation("Created and checked out branch: {BranchName}", branchName);
        return branchName;
    }

    public void CommitChanges(string repoPath, string commitMessage)
    {
        _logger.LogInformation("Committing changes in {RepoPath}", repoPath);
        
        using var repo = new Repository(repoPath);
        
        // Stage all changes
        Commands.Stage(repo, "*");
        
        // Create signature
        var signature = new Signature("Ticket2PR Bot", "ticket2pr@example.com", DateTimeOffset.Now);
        
        // Commit
        repo.Commit(commitMessage, signature, signature);
        
        _logger.LogInformation("Changes committed successfully");
    }

    public void PushBranch(string repoPath, string branchName, string githubToken)
    {
        _logger.LogInformation("Pushing branch {BranchName} from {RepoPath}", branchName, repoPath);
        
        using var repo = new Repository(repoPath);
        
        var options = new PushOptions
        {
            CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
            {
                Username = githubToken,
                Password = string.Empty
            }
        };

        var remote = repo.Network.Remotes["origin"];
        var pushRefSpec = $"refs/heads/{branchName}";
        repo.Network.Push(remote, pushRefSpec, options);
        
        _logger.LogInformation("Branch pushed successfully");
    }

    internal string GetTokenForPush(string token)
    {
        return token;
    }

    private string GenerateBranchName(string ticketDescription)
    {
        // Create a short, unique branch name
        var sanitized = new string(ticketDescription
            .Take(10)
            .Where(c => char.IsLetterOrDigit(c) || c == '-')
            .ToArray())
            .ToLower();
        
        if (string.IsNullOrEmpty(sanitized))
        {
            sanitized = "ticket";
        }
        
        // Add timestamp to make it unique (5 chars for last 5 digits of timestamp)
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var shortTimestamp = timestamp[^5..];
        
        var branchName = $"{sanitized}-{shortTimestamp}";
        
        // Ensure it's max 15 characters
        if (branchName.Length > 15)
        {
            branchName = branchName[..15];
        }
        
        return branchName;
    }
}
