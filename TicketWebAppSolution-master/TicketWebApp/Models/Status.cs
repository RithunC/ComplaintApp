using System.Collections.ObjectModel;

namespace TicketWebApp.Models
{
    public class Status: IComparable<Status>, IEquatable<Status>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;  // New, In Progress, On Hold, Resolved, Closed
        public bool IsClosedState { get; set; } = false;

        // Navigation
        public Collection<Ticket>? Tickets { get; set; }

        public int CompareTo(Status? other) => other != null ? Id.CompareTo(other.Id) : 1;
        public bool Equals(Status? other) => other != null && Id == other.Id;
        public override string ToString() => $"Id: {Id}, Name: {Name}, Closed: {IsClosedState}";
    }
}
