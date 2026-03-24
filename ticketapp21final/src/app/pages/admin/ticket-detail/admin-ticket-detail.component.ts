import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe, LowerCasePipe } from '@angular/common';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { ReplacePipe } from '../../../shared/pipes/replace.pipe';
import { TicketService } from '../../../services/ticket.service';
import { CommentService } from '../../../services/comment.service';
import { AttachmentService } from '../../../services/attachment.service';
import { LookupService } from '../../../services/lookup.service';
import { UserService } from '../../../services/user.service';
import { ToastService } from '../../../services/toast.service';
import {
  TicketResponseDto, CommentResponseDto, AttachmentResponseDto,
  TicketStatusHistoryDto, StatusDto, UserLiteDto
} from '../../../models';

@Component({
  selector: 'app-admin-ticket-detail', standalone: true,
  imports: [RouterLink, FormsModule, NavbarComponent, DatePipe, LowerCasePipe, ReplacePipe],
  templateUrl: './admin-ticket-detail.component.html',
  styleUrls: ['./admin-ticket-detail.component.css']
})
export class AdminTicketDetailComponent implements OnInit {
  route      = inject(ActivatedRoute);
  ts         = inject(TicketService);
  cs         = inject(CommentService);
  attachSvc  = inject(AttachmentService);
  ls         = inject(LookupService);
  us         = inject(UserService);
  toast      = inject(ToastService);

  ticket      = signal<TicketResponseDto | null>(null);
  comments    = signal<CommentResponseDto[]>([]);
  attachments = signal<AttachmentResponseDto[]>([]);
  history     = signal<TicketStatusHistoryDto[]>([]);
  statuses    = signal<StatusDto[]>([]);
  agents      = signal<UserLiteDto[]>([]);
  deletingAttachId = signal<number | null>(null);

  newStatusId = 0; statusNote = ''; assignUserId = 0;
  newComment = ''; isInternal = false;

  get tid(): number { return +this.route.snapshot.paramMap.get('id')!; }

  ngOnInit(): void {
    this.ls.getStatuses().subscribe(s => this.statuses.set(s));
    this.us.getAgents().subscribe(a => this.agents.set(a));
    this.loadAll();
  }

  loadAll(): void {
    this.ts.get(this.tid).subscribe(t => this.ticket.set(t));
    this.cs.getByTicket(this.tid).subscribe(c => this.comments.set(c));
    this.ts.getHistory(this.tid).subscribe(h => this.history.set(h));
    this.attachSvc.getByTicket(this.tid).subscribe(a => this.attachments.set(a));
  }

  updateStatus(): void {
    if (!this.newStatusId) return;
    this.ts.updateStatus(this.tid, { newStatusId: this.newStatusId, note: this.statusNote || undefined })
      .subscribe({ next: () => { this.toast.success('Status updated!'); this.newStatusId = 0; this.statusNote = ''; this.loadAll(); } });
  }

  assignAgent(): void {
    if (!this.assignUserId) return;
    this.ts.assign(this.tid, { assignedToUserId: this.assignUserId })
      .subscribe({ next: () => { this.toast.success('Agent assigned!'); this.assignUserId = 0; this.loadAll(); } });
  }

  addComment(): void {
    if (!this.newComment.trim()) return;
    this.cs.add({ ticketId: this.tid, body: this.newComment.trim(), isInternal: this.isInternal }).subscribe({
      next: () => { this.toast.success('Comment posted!'); this.newComment = ''; this.isInternal = false; this.cs.getByTicket(this.tid).subscribe(c => this.comments.set(c)); }
    });
  }

  onFile(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    this.attachSvc.upload(this.tid, file).subscribe({
      next: () => { this.toast.success(`"${file.name}" uploaded!`); this.attachSvc.getByTicket(this.tid).subscribe(a => this.attachments.set(a)); }
    });
  }

  deleteAttachment(a: AttachmentResponseDto): void {
    if (this.deletingAttachId() !== null) return;
    this.deletingAttachId.set(a.id);
    this.attachSvc.delete(a.id).subscribe({
      next: () => { this.toast.success(`"${a.fileName}" deleted.`); this.attachments.update(list => list.filter(x => x.id !== a.id)); this.deletingAttachId.set(null); },
      error: () => this.deletingAttachId.set(null)
    });
  }

  download(a: AttachmentResponseDto): void {
    this.attachSvc.download(a.id).subscribe(blob => {
      const url = URL.createObjectURL(blob); const link = document.createElement('a');
      link.href = url; link.download = a.fileName; link.click(); URL.revokeObjectURL(url);
    });
  }

  fmtSize(b: number): string {
    if (b < 1024) return `${b} B`;
    if (b < 1048576) return `${(b / 1024).toFixed(1)} KB`;
    return `${(b / 1048576).toFixed(1)} MB`;
  }

  prioBg(p: string): string    { return ({ High: '#dc2626', Urgent: '#7c3aed', Medium: '#d97706', Low: '#059669' } as Record<string,string>)[p] ?? '#475569'; }
  isOverdue(d: string): boolean { return new Date(d) < new Date(); }
}
