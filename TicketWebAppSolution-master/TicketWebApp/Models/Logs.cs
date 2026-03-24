//using System.ComponentModel.DataAnnotations;

//namespace TicketWebApp.Models
//{
//    public class Logs
//    {
//        [Key] 
//        public Guid Id { get; set; } = Guid.NewGuid();

//        [Required][MaxLength(500)] 
//        public string Message { get; set; } = string.Empty;

//        [MaxLength(5000)] 
//        public string StackTrace { get; set; } = string.Empty;

//        [MaxLength(500)] 
//        public string InnerException { get; set; } = string.Empty;

//        [MaxLength(100)] 
//        public string ExceptionType { get; set; } = string.Empty;

//        [MaxLength(100)] 
//        public string UserName { get; set; } = string.Empty;

//        [MaxLength(50)] 
//        public string Role { get; set; } = string.Empty;

//        public Guid? UserId { get; set; }

//        [MaxLength(100)] 
//        public string Controller { get; set; } = string.Empty;

//        [MaxLength(100)] 
//        public string Action { get; set; } = string.Empty;

//        [MaxLength(20)] 
//        public string HttpMethod { get; set; } = string.Empty;

//        [MaxLength(500)] 
//        public string RequestPath { get; set; } = string.Empty;

//        [MaxLength(500)] 
//        public string QueryString { get; set; } = string.Empty;

//        [MaxLength(5000)] 
//        public string RequestBody { get; set; } = string.Empty;

//        public int StatusCode { get; set; }

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

//        [Timestamp] public byte[] RowVersion { get; set; }
    
//    }
//}
