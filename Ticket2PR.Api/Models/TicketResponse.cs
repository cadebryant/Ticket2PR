namespace Ticket2PR.Api.Models;

public class TicketResponse
{
    public required string BranchName { get; set; }
    public required string PullRequestUrl { get; set; }
    public required string Status { get; set; }
    public string? Message { get; set; }
}
