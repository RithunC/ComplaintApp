namespace TicketWebApp.Models.DTOs
{
    public class PriorityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Low, Medium, High, Urgent
        public int Rank { get; set; }                    // 1 highest
        public string ColorHex { get; set; } = "#999999";

    }
}
