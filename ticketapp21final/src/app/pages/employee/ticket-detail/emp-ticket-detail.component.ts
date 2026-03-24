import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe, LowerCasePipe } from '@angular/common';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { ReplacePipe } from '../../../shared/pipes/replace.pipe';
import { TicketService } from '../../../services/ticket.service';
import { CommentService } from '../../../services/comment.service';
import { AttachmentService } from '../../../services/attachment.service';
import { EmployeeTicketService } from '../../../services/employee-ticket.service';
import { AuthService } from '../../../services/auth.service';
import { ToastService } from '../../../services/toast.service';
import {
  TicketResponseDto, CommentResponseDto, AttachmentResponseDto,
  TicketStatusHistoryDto
} from '../../../models';

@Component({
  selector: 'app-emp-ticket-detail',
  standalone: true,
  imports: [RouterLink, FormsModule, NavbarComponent, DatePipe, LowerCasePipe, ReplacePipe],
  templateUrl: './emp-ticket-detail.component.html',
  styleUrls: ['./emp-ticket-detail.component.css']
})
export class EmpTicketDetailComponent implements OnInit {
  route      = inject(ActivatedRoute);
  ts         = inject(TicketService);
  cs         = inject(CommentService);
  attachSvc  = inject(AttachmentService);
  empTs      = inject(EmployeeTicketService);
  auth       = inject(AuthService);
  toast      = inject(ToastService);

  ticket      = signal<TicketResponseDto | null>(null);
  comments    = signal<CommentResponseDto[]>([]);
  attachments = signal<AttachmentResponseDto[]>([]);
  history     = signal<TicketStatusHistoryDto[]>([]);
  newComment  = '';
  deletingAttachId = signal<number | null>(null);

  get tid(): number { return +this.route.snapshot.paramMap.get('id')!; }
  get myId(): number { return this.auth.currentUser()?.id ?? 0; }

  get isClosed(): boolean {
    const s = this.ticket()?.statusName?.toLowerCase() ?? '';
    return s === 'closed' || s === 'resolved';
  }

  ngOnInit(): void { this.loadAll(); }

  loadAll(): void {
    this.ts.get(this.tid).subscribe({
      next: t => {
        this.ticket.set(t);
        if (t) this.empTs.saveTicketId(t.id);
      },
      error: () => this.toast.error('Failed to load ticket.')
    });
    this.cs.getByTicket(this.tid).subscribe(c => this.comments.set(c));
    this.attachSvc.getByTicket(this.tid).subscribe(a => this.attachments.set(a));
    // Backend: GET /tickets/{id}/history  [Authorize(Roles = "Admin,Agent,Employee")]
    // Employee can view history for their OWN ticket — backend checks CreatedByUserId
    this.ts.getHistory(this.tid).subscribe({
      next: h => this.history.set(h),
      error: () => this.history.set([]) // silently ignore if forbidden
    });
  }

  addComment(): void {
    const body = this.newComment.trim();
    if (!body) return;
    this.cs.add({ ticketId: this.tid, body, isInternal: false }).subscribe({
      next: () => {
        this.toast.success('Comment posted!');
        this.newComment = '';
        this.cs.getByTicket(this.tid).subscribe(c => this.comments.set(c));
      },
      error: (err) => {
        const msg = err.error?.message ?? err.error ?? '';
        if (typeof msg === 'string' && msg) this.toast.error(msg);
      }
    });
  }

  onFile(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    this.attachSvc.upload(this.tid, file).subscribe({
      next: () => {
        this.toast.success(`"${file.name}" uploaded!`);
        this.attachSvc.getByTicket(this.tid).subscribe(a => this.attachments.set(a));
      }
    });
  }

  deleteAttachment(a: AttachmentResponseDto): void {
    if (this.deletingAttachId() !== null) return;
    this.deletingAttachId.set(a.id);
    this.attachSvc.delete(a.id).subscribe({
      next: () => {
        this.toast.success(`"${a.fileName}" deleted.`);
        this.attachments.update(list => list.filter(x => x.id !== a.id));
        this.deletingAttachId.set(null);
      },
      error: () => this.deletingAttachId.set(null)
    });
  }

  download(a: AttachmentResponseDto): void {
    this.attachSvc.download(a.id).subscribe(blob => {
      const url  = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url; link.download = a.fileName; link.click();
      URL.revokeObjectURL(url);
    });
  }

  fmtSize(b: number): string {
    if (b < 1024)    return `${b} B`;
    if (b < 1048576) return `${(b / 1024).toFixed(1)} KB`;
    return `${(b / 1048576).toFixed(1)} MB`;
  }

  prioBg(p: string): string    { return ({ High: '#dc2626', Urgent: '#7c3aed', Medium: '#d97706', Low: '#059669' } as Record<string,string>)[p] ?? '#475569'; }
  isOverdue(d: string): boolean { return new Date(d) < new Date(); }
}
