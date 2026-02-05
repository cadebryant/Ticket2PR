# Ticket2PR

Parses a development ticket, applies code changes, and submits a PR

## Overview

Ticket2PR is a C# .NET API application that automates the process of creating pull requests from ticket descriptions. It accepts a ticket description and GitHub repository URL, then automatically:

1. Clones the repository
2. Creates a unique feature branch
3. Implements code changes
4. Commits and pushes the changes
5. Creates a pull request

## Quick Start

Navigate to the API project:
```bash
cd Ticket2PR.Api
```

See [Ticket2PR.Api/README.md](Ticket2PR.Api/README.md) for detailed setup and usage instructions.

## Features

- RESTful API with POST endpoint
- Automatic branch creation with unique names (max 15 characters)
- GitHub integration using Octokit
- Git operations using LibGit2Sharp
- Swagger/OpenAPI documentation

## Architecture

The application is structured as:
- **Models**: Request/Response DTOs
- **Services**: Business logic (Git operations, GitHub API, code generation)
- **Controllers**: API endpoints

## Requirements

- .NET 9.0 SDK
- GitHub Personal Access Token with `repo` scope

## API Endpoint

**POST** `/api/tickets`

Request:
```json
{
  "gitHubUrl": "https://github.com/owner/repository",
  "ticketDescription": "Your ticket description here"
}
```

Response:
```json
{
  "branchName": "feature-12345",
  "pullRequestUrl": "https://github.com/owner/repository/pull/123",
  "status": "Success",
  "message": "Pull request created successfully"
}
```

## License

MIT License
