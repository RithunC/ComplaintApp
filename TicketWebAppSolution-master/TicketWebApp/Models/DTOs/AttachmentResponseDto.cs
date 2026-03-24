namespace TicketWebApp.Models.DTOs
{
    public class AttachmentResponseDto
    {
        public long Id { get; set; }
        public long TicketId { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public string StoragePath { get; set; } = string.Empty;

        public long UploadedByUserId { get; set; }
        public string UploadedByName { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }

    }

    public class AttachmentDownloadResult
    {
        public Stream Stream { get; set; } = Stream.Null;  //efficiently handle file download without loading the entire file into memory
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
        public long FileSizeBytes { get; set; }
    }

}
