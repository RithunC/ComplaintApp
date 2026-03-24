import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe, LowerCasePipe } from '@angular/common';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { ReplacePipe } from '../../../shared/pipes/replace.pipe';
import { EmployeeTicketService } from '../../../services/employee-ticket.service';
import { LookupService } from '../../../services/lookup.service';
import { ToastService } from '../../../services/toast.service';
import { TicketResponseDto, StatusDto } from '../../../models';

@Component({
  selector: 'app-my-tickets',
  standalone: true,
  imports: [RouterLink, FormsModule, NavbarComponent, DatePipe, LowerCasePipe, ReplacePipe],
  templateUrl: './my-tickets.component.html',
  styleUrls: ['./my-tickets.component.css']
})
export class MyTicketsComponent implements OnInit {
  empTs        = inject(EmployeeTicketService);
  ls           = inject(LookupService);
  toast        = inject(ToastService);
  allTickets   = signal<TicketResponseDto[]>([]);
  statuses     = signal<StatusDto[]>([]);
  selected     = signal<string | undefined>(undefined);
  loadingState = signal(false);
  page         = signal(1);
  pageSize     = 10;

  filtered = computed(() => {
    const sel = this.selected();
    const all = this.allTickets();
    if (!sel) return all;
    return all.filter(t => t.statusName?.toLowerCase() === sel.toLowerCase());
  });

  paginated = computed(() => {
    const start = (this.page() - 1) * this.pageSize;
    return this.filtered().slice(start, start + this.pageSize);
  });

  get total(): number { return this.allTickets().length; }
  get totalPages(): number { return Math.ceil(this.filtered().length / this.pageSize); }
  isLastPage(): boolean { return this.page() >= this.totalPages; }

  pageNumbers(): number[] {
    const total = this.totalPages; const cur = this.page();
    if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
    const pages: number[] = [1];
    if (cur > 3) pages.push(-1);
    for (let i = Math.max(2, cur - 1); i <= Math.min(total - 1, cur + 1); i++) pages.push(i);
    if (cur < total - 2) pages.push(-1);
    pages.push(total);
    return pages;
  }

  countByStatus(name: string): number {
    return this.allTickets().filter(t => t.statusName?.toLowerCase() === name.toLowerCase()).length;
  }

  ngOnInit(): void {
    this.ls.getStatuses().subscribe(s => this.statuses.set(s));
    this.load();
  }

  load(): void {
    this.loadingState.set(true);
    this.empTs.getMyTickets().subscribe({
      next: tickets => {
        this.allTickets.set(tickets.sort((a, b) => b.id - a.id));
        this.loadingState.set(false);
      },
      error: () => { this.toast.error('Failed to load tickets.'); this.loadingState.set(false); }
    });
  }

  setStatus(name: string | undefined): void { this.selected.set(name); this.page.set(1); }
  goToPage(p: number): void { this.page.set(p); }
  prevPage(): void { if (this.page() > 1) this.page.update(p => p - 1); }
  nextPage(): void { if (!this.isLastPage()) this.page.update(p => p + 1); }
  isOverdue(d: string): boolean { return new Date(d) < new Date(); }
  prioBg(p: string): string { return ({ High: '#dc2626', Urgent: '#7c3aed', Medium: '#d97706', Low: '#059669' } as Record<string,string>)[p] ?? '#475569'; }
  min(a: number, b: number): number { return Math.min(a, b); }
}
