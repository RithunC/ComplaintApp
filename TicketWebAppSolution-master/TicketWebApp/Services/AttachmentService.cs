using Microsoft.EntityFrameworkCore;
using TicketWebApp.Interfaces;
using TicketWebApp.Models;
using TicketWebApp.Models.DTOs;

namespace TicketWebApp.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IRepository<long, Attachment> _attachRepo;
        private readonly IRepository<long, User> _userRepo;
        private readonly IRepository<long, Ticket> _ticketRepo;

        public AttachmentService(
            IRepository<long, Attachment> attachRepo,
            IRepository<long, User> userRepo,
            IRepository<long, Ticket> ticketRepo)
        {
            _attachRepo = attachRepo;
            _userRepo = userRepo;
            _ticketRepo = ticketRepo;
        }

        public async Task<AttachmentResponseDto> UploadAsync(long ticketId, long uploadedByUserId, IFormFile file)
        {
            if (file == null)
                throw new Exception("No file provided");

            if (ticketId <= 0)
                throw new Exception("Invalid ticket id");

            if (uploadedByUserId <= 0)
                throw new Exception("Invalid user id");

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            fileName = fileName.Replace("/", "_").Replace("\\", "_");

            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relPath = $"/uploads/{fileName}";
            var attachment = new Attachment
            {
                TicketId = ticketId,
                UploadedByUserId = uploadedByUserId,
                FileName = file.FileName,
                ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                    ? "application/octet-stream"
                    : file.ContentType,
                FileSizeBytes = file.Length,
                StoragePath = relPath,
                UploadedAt = DateTime.UtcNow
            };

            await _attachRepo.Add(attachment);

            var by = await _userRepo.Get(uploadedByUserId);
            if (by == null)
                throw new Exception("Uploaded-by user not found");

            return new AttachmentResponseDto
            {
                Id = attachment.Id,
                TicketId = ticketId,
                FileName = attachment.FileName,
                ContentType = attachment.ContentType,
                FileSizeBytes = attachment.FileSizeBytes,
                StoragePath = attachment.StoragePath,
                UploadedByUserId = uploadedByUserId,
                UploadedByName = by.DisplayName,
                UploadedAt = attachment.UploadedAt
            };
        }

        public async Task<IReadOnlyList<AttachmentResponseDto>> GetByTicketAsync(long ticketId)
        {
            if (ticketId <= 0)
                throw new Exception("Invalid ticket id");

            var list = await _attachRepo.GetQueryable()
                .Include(a => a.UploadedBy)
                .Where(a => a.TicketId == ticketId)
                .OrderByDescending(a => a.UploadedAt)
                .ToListAsync();

            return list.Select(a => new AttachmentResponseDto
            {
                Id = a.Id,
                TicketId = a.TicketId,
                FileName = a.FileName,
                ContentType = a.ContentType,
                FileSizeBytes = a.FileSizeBytes,
                StoragePath = a.StoragePath,
                UploadedByUserId = a.UploadedByUserId,
                UploadedByName = a.UploadedBy?.DisplayName ?? "",
                UploadedAt = a.UploadedAt
            }).ToList();
        }

        public async Task<AttachmentDownloadResult?> GetDownloadAsync(long attachmentId, long requestingUserId)
        {
            if (attachmentId <= 0)
                throw new Exception("Invalid attachment id");

            var attachment = await _attachRepo.GetQueryable()
                .Include(a => a.Ticket)
                .FirstOrDefaultAsync(a => a.Id == attachmentId);

            if (attachment == null)
                throw new Exception("Attachment not found");

            

            var fullPath = ResolvePhysicalPath(attachment.StoragePath);
            if (!File.Exists(fullPath))
                throw new Exception("File not found on server");

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return new AttachmentDownloadResult
            {
                Stream = stream,
                FileName = attachment.FileName,
                ContentType = string.IsNullOrWhiteSpace(attachment.ContentType)
                    ? "application/octet-stream"
                    : attachment.ContentType,
                FileSizeBytes = attachment.FileSizeBytes
            };
        }

        public async Task<bool> DeleteAsync(long attachmentId, long requestingUserId)
        {
            if (attachmentId <= 0)
                throw new Exception("Invalid attachment id");

            var attachment = await _attachRepo.Get(attachmentId);
            if (attachment == null)
                throw new Exception("Attachment not found");

            if (!await CanDeleteAttachmentAsync(attachment, requestingUserId))
                throw new UnauthorizedAccessException("Not allowed to delete this attachment");

            // Delete physical file (no console logging)
            var fullPath = ResolvePhysicalPath(attachment.StoragePath);
            if (File.Exists(fullPath))
            {
                try { File.Delete(fullPath); }
                catch { /* ignore physical delete error */ }
            }

            await _attachRepo.Delete(attachmentId);
            return true;
        }

        private string ResolvePhysicalPath(string storagePath)
        {
            var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var relative = storagePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var fullPath = Path.GetFullPath(Path.Combine(wwwroot, relative));

            var rootFull = Path.GetFullPath(wwwroot);
            if (!fullPath.StartsWith(rootFull, StringComparison.Ordinal))
                throw new Exception("Invalid storage path");

            return fullPath;
        }

        private async Task<bool> CanViewTicketAsync(long ticketId, long userId)
        {
            var ticket = await _ticketRepo.Get(ticketId);
            if (ticket == null)
                throw new Exception("Ticket not found");

            return ticket.CreatedByUserId == userId || await IsSupportOrAdmin(userId);
        }

        private async Task<bool> CanDeleteAttachmentAsync(Attachment a, long userId)
        {
            if (a.UploadedByUserId == userId)
                return true;

            var ticket = await _ticketRepo.Get(a.TicketId);
            if (ticket == null)
                throw new Exception("Ticket not found");

            if (ticket.CreatedByUserId == userId)
                return true;

            return await IsSupportOrAdmin(userId);
        }

        private Task<bool> IsSupportOrAdmin(long userId)
        {
            return Task.FromResult(false);
        }
    }
}