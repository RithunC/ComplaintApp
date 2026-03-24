// ── AUTH ────────────────────────────────────────────────
export interface LoginRequest    { userName: string; password: string; }
export interface LoginResponse   { token: string; }
export interface RegisterRequest { userName: string; email: string; displayName: string; password: string; roleName: string; departmentName?: string; }
export interface RegisterResponse { success: boolean; userId?: number; userName?: string; role?: string; token?: string; message?: string; }
export interface ForgotPasswordRequest  { userNameOrEmail: string; }
export interface ForgotPasswordResponse { success: boolean; message?: string; resetToken?: string; }
export interface ResetPasswordRequest   { token: string; newPassword: string; }
export interface BasicResponse          { success: boolean; message?: string; }
export interface TokenPayload           { nameid: string; unique_name: string; role: string; exp: number; }

// ── LOOKUPS ─────────────────────────────────────────────
export interface DepartmentDto { id: number; name: string; }
export interface RoleDto       { id: number; name: string; }
export interface CategoryDto   { id: number; name: string; parentCategoryId?: number; parentCategoryName?: string; }
export interface PriorityDto   { id: number; name: string; rank: number; colorHex: string; }
export interface StatusDto     { id: number; name: string; isClosedState: boolean; }

// ── USERS ───────────────────────────────────────────────
export interface UserLiteDto {
  id: number; displayName: string; userName: string; email?: string;
  phone?: string; roleName: string; departmentId?: number;
  departmentName?: string; isActive: boolean;
}
export interface UpdateProfileDto { displayName?: string; phone?: string; }

// ── TICKETS ─────────────────────────────────────────────
export interface TicketCreateDto {
  departmentId?: number; categoryId?: number; priorityId: number;
  title: string; description?: string; dueAt?: string;
}
export interface TicketUpdateDto {
  departmentId?: number | null; categoryId?: number | null;
  priorityId?: number; title?: string; description?: string; dueAt?: string;
}
export interface TicketQueryDto {
  departmentId?: number; categoryId?: number; priorityId?: number; statusId?: number;
  createdByUserId?: number; assigneeUserId?: number;
  createdFrom?: string; createdTo?: string;
  page: number; pageSize: number; sortBy?: string; desc: boolean;
}
export interface TicketListItemDto {
  id: number; title: string; priority: string; priorityRank: number;
  status: string; department?: string; category?: string;
  createdBy: string; assignee?: string; createdAt: string; dueAt?: string;
}
export interface TicketResponseDto {
  id: number; title: string; description?: string;
  departmentId?: number; departmentName?: string;
  categoryId?: number; categoryName?: string;
  priorityId: number; priorityName: string; priorityRank: number;
  statusId: number; statusName: string;
  createdByUserId: number; createdByUserName: string;
  currentAssigneeUserId?: number; currentAssigneeUserName?: string;
  createdAt: string; updatedAt?: string; dueAt?: string;
}
export interface PagedResult<T> { items: T[]; totalCount: number; page: number; pageSize: number; }
export interface TicketStatusUpdateDto    { newStatusId: number; note?: string; }
export interface TicketAssignRequestDto   { assignedToUserId: number; note?: string; }
export interface TicketAutoAssignRequestDto { departmentId?: number; categoryId?: number; note?: string; }
export interface TicketAssignmentResponseDto {
  id: number; ticketId: number;
  assignedToUserId: number; assignedToName: string;
  assignedByUserId: number; assignedByName: string;
  assignedAt: string; unassignedAt?: string; note?: string;
}
export interface TicketStatusHistoryDto {
  id: number; ticketId: number;
  oldStatusId?: number; oldStatusName?: string;
  newStatusId: number; newStatusName: string;
  changedByUserId: number; changedByUserName: string;
  changedAt: string; note?: string;
}

// ── COMMENTS ────────────────────────────────────────────
export interface CommentCreateDto  { ticketId: number; body: string; isInternal: boolean; }
export interface CommentResponseDto { id: number; ticketId: number; body: string; isInternal: boolean; postedByUserId: number; postedByName: string; createdAt: string; }
export interface CommentEditDto    { body: string; }

// ── ATTACHMENTS ─────────────────────────────────────────
export interface AttachmentResponseDto {
  id: number; ticketId: number; fileName: string; contentType: string;
  fileSizeBytes: number; storagePath: string;
  uploadedByUserId: number; uploadedByName: string; uploadedAt: string;
}

// ── REPORTS ─────────────────────────────────────────────
export interface TicketSummaryDto {
  total: number; open: number; inProgress: number; resolved: number; closed: number;
  highPriority: number; mediumPriority: number; lowPriority: number;
  assignedToMe: number; overdue: number;
}

