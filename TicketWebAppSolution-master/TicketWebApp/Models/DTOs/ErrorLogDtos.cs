namespace TicketWebApp.Models.DTOs
{
    public class ErrorLogDtos
    {

        public class ErrorLogCreateDto
        {
            public string ErrorMessage { get; set; } = string.Empty;
            public int ErrorNumber { get; set; }
        }

        public class ErrorLogResponseDto
        {
            public int ErrorId { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
            public int ErrorNumber { get; set; }
            public DateTime CreatedAt { get; set; }
        }

    }
}
