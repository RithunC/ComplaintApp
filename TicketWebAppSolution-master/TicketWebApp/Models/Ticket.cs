using System.Collections.ObjectModel;

namespace TicketWebApp.Models
{
    public class Ticket: IComparable<Ticket>, IEquatable<Ticket>
    {
        public long Id { get; set; }

        // Foreign keys
        public int? DepartmentId { get; set; } 
        public Department? Department { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public int PriorityId { get; set; } //Priority is mandatory for every ticket to define urgency
        public Priority? Priority { get; set; }

        public int StatusId { get; set; }
        public Status? Status { get; set; }

        public long CreatedByUserId { get; set; }
        public User? CreatedBy { get; set; }

        public long? CurrentAssigneeUserId { get; set; } //initially ticket may be unassigned
        public User? CurrentAssignee { get; set; }

        // Data
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Collection<TicketAssignment>? Assignments { get; set; }
        public Collection<TicketStatusHistory>? StatusHistory { get; set; }
        public Collection<Comment>? Comments { get; set; }
        public Collection<Attachment>? Attachments { get; set; }

        public int CompareTo(Ticket? other) => other != null ? Id.CompareTo(other.Id) : 1;
        public bool Equals(Ticket? other) => other != null && Id == other.Id;
        public override string ToString()
            => $"Id: {Id}, Title: {Title}, Status: {Status?.Name}, Priority: {Priority?.Name}, Assignee: {CurrentAssignee?.DisplayName}";
    }
}
