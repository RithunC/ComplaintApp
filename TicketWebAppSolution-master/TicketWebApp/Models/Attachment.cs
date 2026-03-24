namespace TicketWebApp.Models
{
    public class Attachment: IComparable<Attachment>, IEquatable<Attachment>
    {
        public long Id { get; set; }
        public long TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public long UploadedByUserId { get; set; }
        public User? UploadedBy { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public string StoragePath { get; set; } = string.Empty; // file path or URL
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int CompareTo(Attachment? other) => other != null ? Id.CompareTo(other.Id) : 1;
        public bool Equals(Attachment? other) => other != null && Id == other.Id;
        public override string ToString()
            => $"Id: {Id}, TicketId: {TicketId}, File: {FileName} ({ContentType}), Size: {FileSizeBytes}B";
    }
}
