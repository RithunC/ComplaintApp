namespace TicketWebApp.Models.DTOs
{
    public class TicketSummaryDto
    {
        // Totals
        public int Total { get; set; }
        public int Open { get; set; }
        public int InProgress { get; set; }
        public int Resolved { get; set; }
        public int Closed { get; set; }

        // Priority buckets
        public int HighPriority { get; set; }
        public int MediumPriority { get; set; }
        public int LowPriority { get; set; }

        // “My work” (for Agents/Admins, counts tickets assigned to them)
        public int AssignedToMe { get; set; }

        // SLA / overdue (based on DueAt < now and not Closed/Resolved)
        public int Overdue { get; set; }
    }
}