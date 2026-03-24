import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe, LowerCasePipe } from '@angular/common';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { ReplacePipe } from '../../../shared/pipes/replace.pipe';
import { TicketService } from '../../../services/ticket.service';
import { LookupService } from '../../../services/lookup.service';
import { TicketListItemDto, PagedResult, StatusDto, PriorityDto, DepartmentDto } from '../../../models';

@Component({
  selector: 'app-admin-tickets',
  standalone: true,
  imports: [RouterLink, FormsModule, NavbarComponent, DatePipe, LowerCasePipe, ReplacePipe],
  templateUrl: './admin-tickets.component.html',
  styleUrls: ['./admin-tickets.component.css']
})
export class AdminTicketsComponent implements OnInit {
  ts = inject(TicketService);
  ls = inject(LookupService);

  result      = signal<PagedResult<TicketListItemDto> | null>(null);
  statuses    = signal<StatusDto[]>([]);
  priorities  = signal<PriorityDto[]>([]);
  departments = signal<DepartmentDto[]>([]);
  page        = signal(1);
  pageSize    = 15;

  filters: { statusId?: number; priorityId?: number; departmentId?: number; sortBy: string } = { sortBy: 'createdAt' };

  ngOnInit(): void {
    this.ls.getStatuses().subscribe(s => this.statuses.set(s));
    this.ls.getPriorities().subscribe(p => this.priorities.set(p));
    this.ls.getDepartments().subscribe(d => this.departments.set(d));
    this.load();
  }

  load(): void {
    this.ts.query({ ...this.filters, page: this.page(), pageSize: this.pageSize, desc: true })
      .subscribe(r => this.result.set(r));
  }

  applyFilter(): void { this.page.set(1); this.load(); }
  goToPage(p: number): void { this.page.set(p); this.load(); }
  prevPage(): void { if (this.page() > 1) { this.page.update(p => p - 1); this.load(); } }
  nextPage(): void { if (!this.isLastPage()) { this.page.update(p => p + 1); this.load(); } }

  get totalPages(): number { return Math.ceil((this.result()?.totalCount ?? 0) / this.pageSize); }
  isLastPage(): boolean { return this.page() >= this.totalPages; }
  pageNumbers(): number[] {
    const total = this.totalPages;
    const cur   = this.page();
    if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
    const pages: number[] = [];
    pages.push(1);
    if (cur > 3) pages.push(-1);
    for (let i = Math.max(2, cur - 1); i <= Math.min(total - 1, cur + 1); i++) pages.push(i);
    if (cur < total - 2) pages.push(-1);
    pages.push(total);
    return pages;
  }

  isOverdue(d: string): boolean { return new Date(d) < new Date(); }
  prioBg(p: string): string     { return ({ High: '#dc2626', Urgent: '#7c3aed', Medium: '#d97706', Low: '#059669' } as Record<string,string>)[p] ?? '#475569'; }
  min(a: number, b: number): number { return Math.min(a, b); }
}
