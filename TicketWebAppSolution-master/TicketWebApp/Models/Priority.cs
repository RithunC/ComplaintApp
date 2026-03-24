using System.Collections.ObjectModel;

namespace TicketWebApp.Models
{
    public class Priority: IComparable<Priority>, IEquatable<Priority>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Low, Medium, High, Urgent
        public int Rank { get; set; }                    // 1=highest, 4=lowest
        public string ColorHex { get; set; } = "#999999";

        // Navigation
        public Collection<Ticket>? Tickets { get; set; }

        public int CompareTo(Priority? other) => other != null ? Id.CompareTo(other.Id) : 1;
        public bool Equals(Priority? other) => other != null && Id == other.Id;
        public override string ToString() => $"Id: {Id}, {Name} (Rank {Rank})";
    }

}
