namespace TicketWebApp.Models.DTOs
{
    public class TicketStatusHistoryDto
    {
        public long Id { get; set; }
        public long TicketId { get; set; }

        public int? OldStatusId { get; set; }
        public string? OldStatusName { get; set; }

        public int NewStatusId { get; set; }
        public string NewStatusName { get; set; } = string.Empty;

        public long ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; }
        public string? Note { get; set; }

    }
}
