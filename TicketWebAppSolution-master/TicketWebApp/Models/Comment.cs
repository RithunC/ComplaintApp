namespace TicketWebApp.Models
{
    public class Comment: IComparable<Comment>, IEquatable<Comment>
    {
        public long Id { get; set; }
        public long TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public long PostedByUserId { get; set; }
        public User? PostedBy { get; set; }

        public string Body { get; set; } = string.Empty;
        public bool IsInternal { get; set; } = false; //only visible to staff
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CompareTo(Comment? other) => other != null ? Id.CompareTo(other.Id) : 1;
        public bool Equals(Comment? other) => other != null && Id == other.Id;
        public override string ToString()
            => $"Id: {Id}, TicketId: {TicketId}, By: {PostedBy?.DisplayName}, Internal: {IsInternal}, At: {CreatedAt:u}";
    }

}
