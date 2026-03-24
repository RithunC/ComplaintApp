import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { catchError, of } from 'rxjs';
import { AuthService } from '../../../services/auth.service';
import { LookupService } from '../../../services/lookup.service';
import { ToastService } from '../../../services/toast.service';
import { RoleDto, DepartmentDto } from '../../../models';

// Fallback data shown when API requires auth on the register page
const DEFAULT_ROLES: RoleDto[] = [
  { id: 1, name: 'Admin' },
  { id: 2, name: 'Agent' },
  { id: 3, name: 'Employee' }
];
const DEFAULT_DEPARTMENTS: DepartmentDto[] = [
  { id: 1, name: 'IT Support' }
];

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  auth    = inject(AuthService);
  lookup  = inject(LookupService);
  toast   = inject(ToastService);
  router  = inject(Router);

  loading     = signal(false);
  showPwd     = signal(false);
  roles       = signal<RoleDto[]>(DEFAULT_ROLES);        // pre-filled with defaults
  departments = signal<DepartmentDto[]>(DEFAULT_DEPARTMENTS); // pre-filled with defaults

  form = {
    userName: '',
    email: '',
    displayName: '',
    password: '',
    roleName: '',
    departmentName: ''
  };

  ngOnInit(): void {
    // Try API — silently fall back to defaults on error (API requires auth)
    this.lookup.getRoles().pipe(
      catchError(() => of(DEFAULT_ROLES))
    ).subscribe(r => this.roles.set(r?.length ? r : DEFAULT_ROLES));

    this.lookup.getDepartments().pipe(
      catchError(() => of(DEFAULT_DEPARTMENTS))
    ).subscribe(d => this.departments.set(d?.length ? d : DEFAULT_DEPARTMENTS));
  }

  onSubmit(): void {
    if (this.loading()) return;

    // Client-side validation
    if (!this.form.userName.trim()) { this.toast.error('Username is required.'); return; }
    if (!this.form.email.trim())    { this.toast.error('Email is required.'); return; }
    if (!this.form.displayName.trim()) { this.toast.error('Display name is required.'); return; }
    if (this.form.password.length < 6) { this.toast.error('Password must be at least 6 characters.'); return; }
    if (!this.form.roleName)        { this.toast.error('Please select a role.'); return; }

    this.loading.set(true);

    this.auth.register({
      ...this.form,
      departmentName: this.form.departmentName || undefined
    }).subscribe({
      next: res => {
        this.loading.set(false);
        if (res.success) {
          this.toast.success('Account created successfully! Please sign in.');
          // Redirect to login after short delay
          setTimeout(() => this.router.navigate(['/login']), 1500);
        } else {
          // Show specific backend error (e.g. "Username already exists")
          this.toast.error(res.message ?? 'Registration failed. Please try again.');
        }
      },
      error: (err) => {
        this.loading.set(false);
        const msg = err.error?.message ?? err.error ?? 'Registration failed. Please try again.';
        this.toast.error(typeof msg === 'string' ? msg : 'Registration failed.');
      }
    });
  }

  togglePwd(): void { this.showPwd.update(v => !v); }
}
