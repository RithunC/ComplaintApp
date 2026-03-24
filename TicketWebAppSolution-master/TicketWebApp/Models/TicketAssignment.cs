namespace TicketWebApp.Models
{
    public class TicketAssignment: IComparable<TicketAssignment>, IEquatable<TicketAssignment>
    {
        public long Id { get; set; }

        public long TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public long AssignedToUserId { get; set; }
        public User? AssignedTo { get; set; }

        public long AssignedByUserId { get; set; }
        public User? AssignedBy { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UnassignedAt { get; set; }
        public string? Note { get; set; }

        public int CompareTo(TicketAssignment? other) => other != null ? Id.CompareTo(other.Id) : 1;
        public bool Equals(TicketAssignment? other) => other != null && Id == other.Id;
        public override string ToString()
            => $"Id: {Id}, TicketId: {TicketId}, To: {AssignedTo?.DisplayName}, AssignedAt: {AssignedAt:u}";
    }

}
