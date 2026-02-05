# Ticket2PR API

A C# .NET API application that accepts development tickets and automatically creates pull requests with code changes.

## Features

- **POST Endpoint**: Accepts ticket descriptions and GitHub repository URLs
- **Automated Branch Creation**: Creates uniquely named feature branches (max 15 characters)
- **Code Implementation**: Implements code changes based on ticket descriptions
- **Pull Request Creation**: Automatically creates PRs with the implemented changes

## Architecture

The application consists of the following components:

### Models
- `TicketRequest`: Input model containing GitHub URL and ticket description
- `TicketResponse`: Output model containing branch name, PR URL, and status

### Services
- `GitService`: Handles Git operations (clone, branch, commit, push)
- `GitHubService`: Handles GitHub API operations (PR creation)
- `CodeGenerationService`: Implements code changes based on ticket descriptions

### Controllers
- `TicketsController`: Exposes POST endpoint at `/api/tickets`

## Prerequisites

- .NET 9.0 SDK
- GitHub Personal Access Token with `repo` scope

## Setup

1. Clone the repository:
```bash
git clone https://github.com/cadebryant/Ticket2PR.git
cd Ticket2PR/Ticket2PR.Api
```

2. Configure GitHub Token:

Create a `.env` file or set the configuration in `appsettings.json`:

```json
{
  "GitHub": {
    "Token": "your_github_personal_access_token"
  }
}
```

Or use environment variable:
```bash
export GitHub__Token="your_github_personal_access_token"
```

3. Build the project:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run
```

The API will be available at `http://localhost:5000` (or the port specified in launchSettings.json).

## API Usage

### Endpoint

**POST** `/api/tickets`

### Request Body

```json
{
  "gitHubUrl": "https://github.com/owner/repository",
  "ticketDescription": "Add a new feature that does X, Y, and Z"
}
```

### Response

Success (200 OK):
```json
{
  "branchName": "addnewfea-12345",
  "pullRequestUrl": "https://github.com/owner/repository/pull/123",
  "status": "Success",
  "message": "Pull request created successfully"
}
```

Error (500 Internal Server Error):
```json
{
  "branchName": "",
  "pullRequestUrl": "",
  "status": "Error",
  "message": "Error details..."
}
```

## Example Usage

### Using cURL

```bash
curl -X POST http://localhost:5000/api/tickets \
  -H "Content-Type: application/json" \
  -d '{
    "gitHubUrl": "https://github.com/owner/repository",
    "ticketDescription": "Implement user authentication feature"
  }'
```

### Using PowerShell

```powershell
$body = @{
    gitHubUrl = "https://github.com/owner/repository"
    ticketDescription = "Implement user authentication feature"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/tickets" -Method Post -Body $body -ContentType "application/json"
```

## Swagger Documentation

When running in development mode, Swagger UI is available at:
- `http://localhost:5000/swagger`

## Workflow

1. **Receive Request**: API receives POST request with GitHub URL and ticket description
2. **Clone Repository**: Repository is cloned to a temporary directory
3. **Create Branch**: A unique feature branch is created (max 15 characters)
4. **Implement Changes**: Code changes are implemented based on the ticket
5. **Commit Changes**: Changes are committed to the feature branch
6. **Push Branch**: Branch is pushed to the remote repository
7. **Create PR**: A pull request is created from the feature branch to main
8. **Return Response**: API returns the branch name and PR URL

## Branch Naming Convention

Branches are automatically named with the following format:
- First 10 characters of sanitized ticket description
- Hyphen separator
- Last 5 digits of Unix timestamp

Example: `implement-54321`

Maximum length: 15 characters

## Dependencies

- **LibGit2Sharp** (0.31.0): Git operations
- **Octokit** (14.0.0): GitHub API interactions
- **Swashbuckle.AspNetCore** (10.1.1): Swagger/OpenAPI documentation

## Security Considerations

- Store GitHub token securely (use Azure Key Vault, AWS Secrets Manager, or environment variables)
- Never commit tokens to source control
- Use `.env` file for local development (already in .gitignore)
- Implement proper authentication/authorization for the API endpoint in production
- Consider rate limiting and request validation

## Future Enhancements

- Integration with AI/LLM for actual code generation
- Support for multiple base branches (not just main)
- Webhook support for ticket systems (Jira, GitHub Issues)
- Enhanced error handling and retry logic
- Unit and integration tests
- Docker support
- CI/CD pipeline integration

## License

[Add your license here]

## Contributing

[Add contribution guidelines here]
