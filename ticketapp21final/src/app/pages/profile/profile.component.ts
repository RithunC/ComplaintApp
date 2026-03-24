import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LowerCasePipe } from '@angular/common';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import { UserLiteDto } from '../../models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [FormsModule, NavbarComponent, LowerCasePipe],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  userSvc = inject(UserService);
  auth    = inject(AuthService);
  toast   = inject(ToastService);

  profile  = signal<UserLiteDto | null>(null);
  editing  = signal(false);
  saving   = signal(false);

  editForm = { displayName: '', phone: '' };

  initials(): string {
    const n = this.profile()?.displayName ?? '';
    return n.split(' ').map(x => x[0]).slice(0,2).join('').toUpperCase() || 'U';
  }

  ngOnInit(): void { this.loadProfile(); }

  loadProfile(): void {
    this.userSvc.getMe().subscribe({
      next: u => {
        this.profile.set(u);
        this.auth.currentUser.update(cur =>
          cur ? { ...cur, displayName: u.displayName } : cur
        );
      },
      error: () => this.toast.error('Failed to load profile.')
    });
  }

  startEdit(): void {
    const p = this.profile();
    if (!p) return;
    this.editForm.displayName = p.displayName;
    this.editForm.phone = p.phone ?? '';
    this.editing.set(true);
  }

  cancelEdit(): void { this.editing.set(false); }

  saveEdit(): void {
    if (this.saving()) return;
    if (!this.editForm.displayName.trim()) {
      this.toast.error('Display name cannot be empty.');
      return;
    }
    this.saving.set(true);
    this.userSvc.updateMe({
      displayName: this.editForm.displayName.trim(),
      phone: this.editForm.phone.trim() || undefined
    }).subscribe({
      next: updated => {
        this.profile.set(updated);
        this.auth.currentUser.update(cur =>
          cur ? { ...cur, displayName: updated.displayName } : cur
        );
        this.editing.set(false);
        this.saving.set(false);
        this.toast.success('Profile updated successfully!');
      },
      error: () => { this.saving.set(false); }
    });
  }
}
