import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  LoginRequest, LoginResponse, RegisterRequest, RegisterResponse,
  ForgotPasswordRequest, ForgotPasswordResponse,
  ResetPasswordRequest, BasicResponse, TokenPayload, UserLiteDto
} from '../models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TK  = 'jwt_token';
  private readonly UK  = 'current_user';
  private readonly EMP = 'emp_ticket_ids';   // clear on logout

  token       = signal<string | null>(localStorage.getItem(this.TK));
  currentUser = signal<UserLiteDto | null>(this._load());

  isAuthenticated = computed(() => !!this.token());
  role            = computed(() => this.currentUser()?.roleName ?? null);
  isAdmin         = computed(() => this.role() === 'Admin');
  isAgent         = computed(() => this.role() === 'Agent');
  isEmployee      = computed(() => this.role() === 'Employee');

  constructor(private http: HttpClient, private router: Router) {}

  login(dto: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/authentication/login`, dto);
  }

  /**
   * Stores JWT, decodes it, sets signals.
   * Returns the decoded role string so the caller can redirect immediately.
   */
  handleLoginSuccess(token: string): string {
    localStorage.setItem(this.TK, token);
    this.token.set(token);
    const p   = this._decode(token);
    const usr: UserLiteDto = {
      id: +p.nameid, userName: p.unique_name,
      displayName: p.unique_name, roleName: p.role, isActive: true
    };
    localStorage.setItem(this.UK, JSON.stringify(usr));
    this.currentUser.set(usr);
    return p.role;
  }

  navigateToRole(role: string): void {
    if (role === 'Admin')      this.router.navigate(['/admin/dashboard']);
    else if (role === 'Agent') this.router.navigate(['/agent/dashboard']);
    else                       this.router.navigate(['/employee/dashboard']);
  }

  redirectByRole(): void { this.navigateToRole(this.role() ?? ''); }

  register(dto: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${environment.apiUrl}/authentication/register`, dto);
  }

  forgotPassword(dto: ForgotPasswordRequest): Observable<ForgotPasswordResponse> {
    return this.http.post<ForgotPasswordResponse>(`${environment.apiUrl}/authentication/forgot-password`, dto);
  }

  resetPassword(dto: ResetPasswordRequest): Observable<BasicResponse> {
    return this.http.post<BasicResponse>(`${environment.apiUrl}/authentication/reset-password`, dto);
  }

  logout(): void {
    localStorage.removeItem(this.TK);
    localStorage.removeItem(this.UK);
    // NOTE: Keep emp_ticket_ids so tickets survive logout/login of SAME user.
    // If you want to clear them on logout uncomment:
    // localStorage.removeItem(this.EMP);
    this.token.set(null);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null { return localStorage.getItem(this.TK); }

  private _decode(token: string): TokenPayload {
    return JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
  }
  private _load(): UserLiteDto | null {
    const r = localStorage.getItem(this.UK);
    return r ? JSON.parse(r) : null;
  }
}
