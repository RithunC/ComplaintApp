import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LowerCasePipe } from '@angular/common';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { UserService } from '../../../services/user.service';
import { UserLiteDto } from '../../../models';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [FormsModule, NavbarComponent, LowerCasePipe],
  templateUrl: './admin-users.component.html',
  styleUrls: ['./admin-users.component.css']
})
export class AdminUsersComponent implements OnInit {
  userSvc  = inject(UserService);
  allUsers = signal<UserLiteDto[]>([]);
  search   = signal('');
  page     = signal(1);
  pageSize = 15;

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    return this.allUsers().filter(u =>
      !q || u.displayName.toLowerCase().includes(q) ||
            u.userName.toLowerCase().includes(q) ||
            u.roleName.toLowerCase().includes(q)
    );
  });

  paginated = computed(() => {
    const start = (this.page() - 1) * this.pageSize;
    return this.filtered().slice(start, start + this.pageSize);
  });

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

  ngOnInit(): void {
    this.userSvc.getAgents().subscribe(agents => this.allUsers.set(agents));
  }

  goToPage(p: number): void { this.page.set(p); }
  prevPage(): void { if (this.page() > 1) this.page.update(p => p - 1); }
  nextPage(): void { if (!this.isLastPage()) this.page.update(p => p + 1); }
  onSearch(): void { this.page.set(1); }
  min(a: number, b: number): number { return Math.min(a, b); }
}
