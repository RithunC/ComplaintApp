import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { TicketService } from '../../../services/ticket.service';
import { AttachmentService } from '../../../services/attachment.service';
import { LookupService } from '../../../services/lookup.service';
import { ToastService } from '../../../services/toast.service';
import { EmployeeTicketService } from '../../../services/employee-ticket.service';
import { DepartmentDto, CategoryDto, PriorityDto } from '../../../models';

@Component({
  selector: 'app-create-ticket',
  standalone: true,
  imports: [RouterLink, FormsModule, NavbarComponent],
  templateUrl: './create-ticket.component.html',
  styleUrls: ['./create-ticket.component.css']
})
export class CreateTicketComponent implements OnInit {
  router    = inject(Router);
  ts        = inject(TicketService);
  attachSvc = inject(AttachmentService);
  ls        = inject(LookupService);
  toast     = inject(ToastService);
  empTs     = inject(EmployeeTicketService);

  loading       = signal(false);
  uploadingFiles = signal(false);
  depts         = signal<DepartmentDto[]>([]);
  cats          = signal<CategoryDto[]>([]);
  prios         = signal<PriorityDto[]>([]);
  selectedFiles = signal<File[]>([]);

  form = {
    title:        '',
    description:  '',
    departmentId: undefined as number | undefined,
    categoryId:   undefined as number | undefined,
    priorityId:   0,
    dueAt:        ''
  };

  get titleLen(): number { return this.form.title.length; }
  get totalSize(): string {
    const bytes = this.selectedFiles().reduce((s, f) => s + f.size, 0);
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1048576) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / 1048576).toFixed(1)} MB`;
  }

  ngOnInit(): void {
    this.ls.getDepartments().subscribe(d => this.depts.set(d));
    this.ls.getCategories().subscribe(c => this.cats.set(c));
    this.ls.getPriorities().subscribe(p => this.prios.set(p));
  }

  onFileSelect(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    const newFiles = Array.from(input.files);
    this.selectedFiles.update(existing => {
      const names = existing.map(f => f.name);
      const unique = newFiles.filter(f => !names.includes(f.name));
      return [...existing, ...unique];
    });
    input.value = ''; // reset input so same file can be added again
  }

  removeFile(index: number): void {
    this.selectedFiles.update(files => files.filter((_, i) => i !== index));
  }

  fmtSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1048576) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / 1048576).toFixed(1)} MB`;
  }

  onSubmit(): void {
    if (this.loading() || !this.form.priorityId || !this.form.title.trim()) return;
    this.loading.set(true);

    this.ts.create({
      title:        this.form.title.trim(),
      description:  this.form.description.trim() || undefined,
      departmentId: this.form.departmentId,
      categoryId:   this.form.categoryId,
      priorityId:   this.form.priorityId,
      dueAt:        this.form.dueAt || undefined
    }).subscribe({
      next: ticket => {
        // Save ticket ID so employee can retrieve it later
        this.empTs.saveTicketId(ticket.id);

        const files = this.selectedFiles();
        if (files.length === 0) {
          this.loading.set(false);
          this.toast.success('Ticket submitted successfully!');
          this.router.navigate(['/employee/tickets', ticket.id]);
          return;
        }

        // Upload attachments sequentially
        this.uploadingFiles.set(true);
        let completed = 0;
        let failed = 0;

        const uploadNext = (index: number): void => {
          if (index >= files.length) {
            this.loading.set(false);
            this.uploadingFiles.set(false);
            if (failed > 0) {
              this.toast.warning(`Ticket created! ${completed} file(s) uploaded, ${failed} failed.`);
            } else {
              this.toast.success(`Ticket created with ${completed} attachment(s)!`);
            }
            this.router.navigate(['/employee/tickets', ticket.id]);
            return;
          }

          this.attachSvc.upload(ticket.id, files[index]).subscribe({
            next: () => { completed++; uploadNext(index + 1); },
            error: () => { failed++; uploadNext(index + 1); }
          });
        };

        uploadNext(0);
      },
      error: () => this.loading.set(false)
    });
  }
}
