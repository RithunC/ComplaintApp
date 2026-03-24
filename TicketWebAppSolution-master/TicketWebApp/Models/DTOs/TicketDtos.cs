namespace TicketWebApp.Models.DTOs
{
    public class TicketDtos
    {

        public class TicketCreateDto
        {
            public int? DepartmentId { get; set; }
            public int? CategoryId { get; set; }
            public int PriorityId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public DateTime? DueAt { get; set; }

            // Optional: force assign (if present, skip auto-assign)
            
        }

        public class TicketUpdateDto
        {
            public int? DepartmentId { get; set; }
            public int? CategoryId { get; set; }
            public int? PriorityId { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public DateTime? DueAt { get; set; }
        }

        public class TicketQueryDto
        {
            public int? DepartmentId { get; set; }
            public int? CategoryId { get; set; }
            public int? PriorityId { get; set; }
            public int? StatusId { get; set; }
            public long? CreatedByUserId { get; set; }
            public long? AssigneeUserId { get; set; }
            public DateTime? CreatedFrom { get; set; }
            public DateTime? CreatedTo { get; set; }

            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 20;
            public string? SortBy { get; set; }   // "CreatedAt","Priority","DueAt"
            public bool Desc { get; set; } = true;
        }

        public class TicketListItemDto
        {
            public long Id { get; set; }
            public string Title { get; set; } = string.Empty;

            public string Priority { get; set; } = string.Empty;
            public int PriorityRank { get; set; }

            public string Status { get; set; } = string.Empty;

            public string? Department { get; set; }
            public string? Category { get; set; }

            public string CreatedBy { get; set; } = string.Empty;
            public string? Assignee { get; set; }

            public DateTime CreatedAt { get; set; }
            public DateTime? DueAt { get; set; }
        }

        public class TicketResponseDto
        {
            public long Id { get; set; }

            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }

            public int? DepartmentId { get; set; }
            public string? DepartmentName { get; set; }

            public int? CategoryId { get; set; }
            public string? CategoryName { get; set; }

            public int PriorityId { get; set; }
            public string PriorityName { get; set; } = string.Empty;

            public int PriorityRank { get; set; }

            public int StatusId { get; set; }
            public string StatusName { get; set; } = string.Empty;

            public long CreatedByUserId { get; set; }
            public string CreatedByUserName { get; set; } = string.Empty;

            public long? CurrentAssigneeUserId { get; set; }
            public string? CurrentAssigneeUserName { get; set; }

            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DueAt { get; set; }
        }

    }
}
