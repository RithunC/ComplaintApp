using System.Collections.ObjectModel;

namespace TicketWebApp.Models
{
    public class Department : IComparable<Department>, IEquatable<Department>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g., IT, Networking, Hardware

        // Navigation
        public Collection<User>? Users { get; set; }
        public Collection<Ticket>? Tickets { get; set; }

        public int CompareTo(Department? other) => other != null ? Id.CompareTo(other.Id) : 1;
        public bool Equals(Department? other) => other != null && Id == other.Id;

        public override string ToString() => $"Id: {Id}, Name: {Name}";
    }


}

