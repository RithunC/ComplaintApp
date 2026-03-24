namespace TicketWebApp.Models.DTOs
{
    public class StatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // New, In Progress, Resolved, Closed
        public bool IsClosedState { get; set; }

    }
}
