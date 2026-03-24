using System.Collections.ObjectModel;

namespace TicketWebApp.Models
{
    public class Category : IComparable<Category>, IEquatable<Category>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;       // e.g., Network, Software, Hardware
        public string? Description { get; set; }

        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public Collection<Category>? Children { get; set; }

        // Navigation
        public Collection<Ticket>? Tickets { get; set; }

        public int CompareTo(Category? other) => other != null ? Id.CompareTo(other.Id) : 1;
        public bool Equals(Category? other) => other != null && Id == other.Id;
        public override string ToString() => $"Id: {Id}, Name: {Name}, ParentId: {ParentCategoryId}";
    }

}