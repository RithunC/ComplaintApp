namespace TicketWebApp.Models
{
    public class TicketStatusHistory : IComparable<TicketStatusHistory>, IEquatable<TicketStatusHistory>
    {
        public long Id { get; set; }

        public long TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public int? OldStatusId { get; set; }
        public Status? OldStatus { get; set; }

        public int NewStatusId { get; set; }
        public Status? NewStatus { get; set; }

        public long ChangedByUserId { get; set; }
        public User? ChangedBy { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string? Note { get; set; }

        public int CompareTo(TicketStatusHistory? other) => other != null ? Id.CompareTo(other.Id) : 1;
        public bool Equals(TicketStatusHistory? other) => other != null && Id == other.Id;
        public override string ToString()
            => $"Id: {Id}, TicketId: {TicketId}, {OldStatus?.Name} -> {NewStatus?.Name} at {ChangedAt:u}";
    }
}
