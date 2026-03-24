import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const auth   = inject(AuthService);
  const router = inject(Router);
  if (auth.isAuthenticated()) return true;
  router.navigate(['/login']);
  return false;
};

export const adminGuard: CanActivateFn = () => {
  const auth   = inject(AuthService);
  const router = inject(Router);
  if (auth.isAdmin()) return true;
  // If logged in but wrong role, redirect to their dashboard
  if (auth.isAuthenticated()) { auth.redirectByRole(); return false; }
  router.navigate(['/login']);
  return false;
};

export const agentGuard: CanActivateFn = () => {
  const auth   = inject(AuthService);
  const router = inject(Router);
  if (auth.isAgent() || auth.isAdmin()) return true;
  if (auth.isAuthenticated()) { auth.redirectByRole(); return false; }
  router.navigate(['/login']);
  return false;
};

export const employeeGuard: CanActivateFn = () => {
  const auth   = inject(AuthService);
  const router = inject(Router);
  if (auth.isEmployee()) return true;
  if (auth.isAuthenticated()) { auth.redirectByRole(); return false; }
  router.navigate(['/login']);
  return false;
};

/**
 * Prevents logged-in users from accessing /login and /register.
 * NOTE: We do NOT use this on login/register routes anymore —
 * the login component handles its own redirect after successful login.
 * This guard only protects against manual URL navigation.
 */
export const guestGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  if (!auth.isAuthenticated()) return true;
  // Already logged in — redirect to their dashboard
  auth.redirectByRole();
  return false;
};
