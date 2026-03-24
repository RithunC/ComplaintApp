using TicketWebApp.Models;
using Microsoft.EntityFrameworkCore;


namespace TicketWebApp.Contexts
{
    public class ComplaintContext : DbContext
    {
        public ComplaintContext(DbContextOptions<ComplaintContext> options) : base(options) // db configuration(sql sever/postgresql),connection string
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketAssignment> TicketAssignments { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<TicketStatusHistory> TicketStatusHistories { get; set; }
        public DbSet<ErrorLog> Errorlogs { get; set; }
        //public DbSet<Logs> logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===== Department =====
            modelBuilder.Entity<Department>()
                .HasKey(d => d.Id).HasName("PK_Departments");

            modelBuilder.Entity<Department>()
                .Property(d => d.Name).HasMaxLength(150).IsRequired();

            // ===== Role =====
            modelBuilder.Entity<Role>()
                .HasKey(r => r.Id).HasName("PK_Roles");

            modelBuilder.Entity<Role>()
                .Property(r => r.Name).HasMaxLength(100).IsRequired();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name).IsUnique().HasDatabaseName("UQ_Role_Name");

            // ===== User =====
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id).HasName("PK_Users");

            modelBuilder.Entity<User>()
                .Property(u => u.UserName).HasMaxLength(100).IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Email).HasMaxLength(256).IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.DisplayName).HasMaxLength(150).IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Phone).HasMaxLength(30);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName).IsUnique().HasDatabaseName("UQ_User_UserName");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique().HasDatabaseName("UQ_User_Email");

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_User_Role");

            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_User_Department");

            // ===== Category (self-referencing) =====
            modelBuilder.Entity<Category>()
                .HasKey(c => c.Id).HasName("PK_Categories");

            modelBuilder.Entity<Category>()
                .Property(c => c.Name).HasMaxLength(150).IsRequired();

            modelBuilder.Entity<Category>()
                .Property(c => c.Description).HasMaxLength(400);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(p => p.Children!)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Category_Parent");

            // ===== Priority =====
            modelBuilder.Entity<Priority>()
                .HasKey(p => p.Id).HasName("PK_Priorities");

            modelBuilder.Entity<Priority>()
                .Property(p => p.Name).HasMaxLength(50).IsRequired();

            modelBuilder.Entity<Priority>()
                .Property(p => p.ColorHex).HasMaxLength(7).IsRequired();

            modelBuilder.Entity<Priority>()
                .HasIndex(p => p.Name).IsUnique().HasDatabaseName("UQ_Priority_Name");

            modelBuilder.Entity<Priority>()
                .HasIndex(p => p.Rank).IsUnique().HasDatabaseName("UQ_Priority_Rank");

            // ===== Status =====
            modelBuilder.Entity<Status>()
                .HasKey(s => s.Id).HasName("PK_Statuses");

            modelBuilder.Entity<Status>()
                .Property(s => s.Name).HasMaxLength(50).IsRequired();

            modelBuilder.Entity<Status>()
                .HasIndex(s => s.Name).IsUnique().HasDatabaseName("UQ_Status_Name");

            // ===== Ticket =====
            modelBuilder.Entity<Ticket>()
                .HasKey(t => t.Id).HasName("PK_Tickets");

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Title).HasMaxLength(200).IsRequired();

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Department)
                .WithMany(d => d.Tickets!)
                .HasForeignKey(t => t.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ticket_Department");

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tickets!)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ticket_Category");

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Priority)
                .WithMany(p => p.Tickets!)
                .HasForeignKey(t => t.PriorityId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ticket_Priority");

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Status)
                .WithMany(s => s.Tickets!)
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ticket_Status");

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.CreatedTickets!)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ticket_CreatedBy");

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CurrentAssignee)
                .WithMany(u => u.AssignedTickets!)
                .HasForeignKey(t => t.CurrentAssigneeUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ticket_CurrentAssignee");

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.StatusId).HasDatabaseName("IX_Ticket_Status");
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.PriorityId).HasDatabaseName("IX_Ticket_Priority");
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.CurrentAssigneeUserId).HasDatabaseName("IX_Ticket_Assignee");
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.DepartmentId).HasDatabaseName("IX_Ticket_Department");
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.CreatedAt).HasDatabaseName("IX_Ticket_CreatedAt");

            // ===== TicketAssignment =====
            modelBuilder.Entity<TicketAssignment>()
                .HasKey(a => a.Id).HasName("PK_TicketAssignments");

            modelBuilder.Entity<TicketAssignment>()
                .Property(a => a.Note).HasMaxLength(400);

            modelBuilder.Entity<TicketAssignment>()
                .HasOne(a => a.Ticket)
                .WithMany(t => t.Assignments!)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Assignment_Ticket");

            modelBuilder.Entity<TicketAssignment>()
                .HasOne(a => a.AssignedTo)
                .WithMany(u => u.AssignmentHistory!)
                .HasForeignKey(a => a.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Assignment_AssignedTo");

            modelBuilder.Entity<TicketAssignment>()
                .HasOne(a => a.AssignedBy)
                .WithMany()
                .HasForeignKey(a => a.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Assignment_AssignedBy");

            modelBuilder.Entity<TicketAssignment>()
                .HasIndex(a => new { a.TicketId, a.UnassignedAt }) //composite index
                .HasDatabaseName("IX_Assignment_Ticket_Open");

            // ===== Comment =====
            modelBuilder.Entity<Comment>()
                .HasKey(c => c.Id).HasName("PK_Comments");

            modelBuilder.Entity<Comment>()
                .Property(c => c.Body).IsRequired();

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Ticket)
                .WithMany(t => t.Comments!)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Comment_Ticket");

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.PostedBy)
                .WithMany()
                .HasForeignKey(c => c.PostedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Comment_PostedBy");

            modelBuilder.Entity<Comment>()
                .HasIndex(c => new { c.TicketId, c.CreatedAt })
                .HasDatabaseName("IX_Comment_Ticket_CreatedAt");

            // ===== Attachment =====
            modelBuilder.Entity<Attachment>()
                .HasKey(a => a.Id).HasName("PK_Attachments");

            modelBuilder.Entity<Attachment>()
                .Property(a => a.FileName).HasMaxLength(260).IsRequired();

            modelBuilder.Entity<Attachment>()
                .Property(a => a.ContentType).HasMaxLength(100).IsRequired();

            modelBuilder.Entity<Attachment>()
                .Property(a => a.StoragePath).HasMaxLength(500).IsRequired();

            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.Ticket)
                .WithMany(t => t.Attachments!)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Attachment_Ticket");

            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.UploadedBy)
                .WithMany()
                .HasForeignKey(a => a.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Attachment_UploadedBy");

            modelBuilder.Entity<Attachment>()
                .HasIndex(a => a.TicketId).HasDatabaseName("IX_Attachment_Ticket");

            // ===== TicketStatusHistory =====
            modelBuilder.Entity<TicketStatusHistory>()
                .HasKey(h => h.Id).HasName("PK_TicketStatusHistory");

            modelBuilder.Entity<TicketStatusHistory>()
                .Property(h => h.Note).HasMaxLength(400);

            modelBuilder.Entity<TicketStatusHistory>()
                .HasOne(h => h.Ticket)
                .WithMany(t => t.StatusHistory!)
                .HasForeignKey(h => h.TicketId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_StatusHistory_Ticket");

            modelBuilder.Entity<TicketStatusHistory>()
                .HasOne(h => h.OldStatus)
                .WithMany()
                .HasForeignKey(h => h.OldStatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_StatusHistory_OldStatus");

            modelBuilder.Entity<TicketStatusHistory>()
                .HasOne(h => h.NewStatus)
                .WithMany()
                .HasForeignKey(h => h.NewStatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_StatusHistory_NewStatus");

            modelBuilder.Entity<TicketStatusHistory>()
                .HasOne(h => h.ChangedBy)
                .WithMany()
                .HasForeignKey(h => h.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_StatusHistory_ChangedBy");

            modelBuilder.Entity<TicketStatusHistory>()
                .HasIndex(h => new { h.TicketId, h.ChangedAt })
                .HasDatabaseName("IX_StatusHistory_Ticket_ChangedAt");
        }
    }
}
