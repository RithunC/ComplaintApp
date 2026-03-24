namespace TicketWebApp.Models.DTOs
{
    public class CommentDtos
    {

        public class CommentCreateDto
        {
            public long TicketId { get; set; }
            public string Body { get; set; } = string.Empty;
            public bool IsInternal { get; set; } = false;
        }

        public class CommentResponseDto
        {
            public long Id { get; set; }
            public long TicketId { get; set; }
            public string Body { get; set; } = string.Empty;
            public bool IsInternal { get; set; }
            public long PostedByUserId { get; set; }
            public string PostedByName { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
        }

        public class CommentEditDto
        {
            [System.ComponentModel.DataAnnotations.Required]
            public string Body { get; set; } = string.Empty;
        }


    }
}
