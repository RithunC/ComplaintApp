import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  auth    = inject(AuthService);
  toast   = inject(ToastService);
  loading = signal(false);
  showPwd = signal(false);
  form    = { userName: '', password: '' };

  onSubmit(): void {
    if (this.loading()) return;

    if (!this.form.userName.trim() || !this.form.password.trim()) {
      this.toast.error('Please enter your username and password.');
      return;
    }

    this.loading.set(true);

    this.auth.login(this.form).subscribe({
      next: (res) => {
        if (!res.token) {
          this.toast.error('Invalid username or password.');
          this.loading.set(false);
          return;
        }

        // Store token and get role back synchronously
        const role = this.auth.handleLoginSuccess(res.token);

        // Show success toast
        this.toast.success(`Welcome back! Redirecting to ${role} dashboard…`);

        // Navigate based on role — slight delay so toast is visible
        setTimeout(() => {
          this.auth.navigateToRole(role);
        }, 600);
      },
      error: (err) => {
        this.loading.set(false);
        // Backend returns 401 for wrong credentials
        if (err.status === 401) {
          this.toast.error('Invalid username or password. Please try again.');
        } else {
          this.toast.error('Login failed. Please check your connection and try again.');
        }
      }
    });
  }

  togglePwd(): void { this.showPwd.update(v => !v); }
}
