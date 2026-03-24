using System.Collections.ObjectModel;

namespace TicketWebApp.Models
{
    public class User : IComparable<User>, IEquatable<User>
    {
        internal DateTime CreatedAt;

        public long Id { get; set; }
        public string UserName { get; set; } = string.Empty;  // unique
        public string Email { get; set; } = string.Empty;     // unique
        public string DisplayName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;

        // Simple auth fields (store hash+salt only)
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        // FK: Role (single role per user to keep it simple)
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        // FK: Department (optional)
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        // Navigation
        public Collection<Ticket>? CreatedTickets { get; set; }
        public Collection<Ticket>? AssignedTickets { get; set; }
        public Collection<TicketAssignment>? AssignmentHistory { get; set; }


        public int CompareTo(User? other) => other != null ? Id.CompareTo(other.Id) : 1;
        public bool Equals(User? other) => other != null && Id == other.Id;
        public override string ToString()
            => $"Id: {Id}, UserName: {UserName}, Email: {Email}, Role: {Role?.Name}, Active: {IsActive}";
    }

}
