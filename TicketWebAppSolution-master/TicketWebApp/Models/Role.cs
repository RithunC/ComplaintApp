using System.Collections.ObjectModel;

namespace TicketWebApp.Models
{
    public class Role : IComparable<Role>, IEquatable<Role>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Admin, Agent, Employee

        // Navigation
        public Collection<User>? Users { get; set; }

        public int CompareTo(Role? other) => other != null ? Id.CompareTo(other.Id) : 1;
        public bool Equals(Role? other) => other != null && Id == other.Id;
        public override string ToString() => $"Id: {Id}, Name: {Name}";
    }
}


