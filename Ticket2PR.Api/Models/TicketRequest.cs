namespace Ticket2PR.Api.Models;

public class TicketRequest
{
    public required string GitHubUrl { get; set; }
    public required string TicketDescription { get; set; }
}
