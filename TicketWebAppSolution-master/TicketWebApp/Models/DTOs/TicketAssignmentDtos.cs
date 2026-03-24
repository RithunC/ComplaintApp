namespace TicketWebApp.Models.DTOs
{
    public class TicketAssignmentDtos
    {
        public class TicketAssignRequestDto
        {
            public long AssignedToUserId { get; set; }
            public string? Note { get; set; }
        }

        public class TicketAutoAssignRequestDto
        {
            // Optional override department/category if needed
            public int? DepartmentId { get; set; }
            public int? CategoryId { get; set; }
            public string? Note { get; set; }
        }

        public class TicketAssignmentResponseDto
        {
            public long Id { get; set; }
            public long TicketId { get; set; }

            public long AssignedToUserId { get; set; }
            public string AssignedToName { get; set; } = string.Empty;

            public long AssignedByUserId { get; set; }
            public string AssignedByName { get; set; } = string.Empty;

            public DateTime AssignedAt { get; set; }
            public DateTime? UnassignedAt { get; set; }
            public string? Note { get; set; }
        }
        }
    }
