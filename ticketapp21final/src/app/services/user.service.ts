import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { UserLiteDto, UpdateProfileDto } from '../models';

@Injectable({ providedIn: 'root' })
export class UserService {
  private base = `${environment.apiUrl}/users`;
  constructor(private http: HttpClient) {}

  /** GET /users/{id} — get any user by ID (Admin) */
  getById(id: number): Observable<UserLiteDto> {
    return this.http.get<UserLiteDto>(`${this.base}/${id}`);
  }

  /** GET /users/me — get current user's own profile */
  getMe(): Observable<UserLiteDto> {
    return this.http.get<UserLiteDto>(`${this.base}/me`);
  }

  /** PUT /users/me — update own profile (displayName, phone) */
  updateMe(dto: UpdateProfileDto): Observable<UserLiteDto> {
    return this.http.put<UserLiteDto>(`${this.base}/me`, dto);
  }

  /** GET /users/agents — get all active agents, optionally filtered by dept */
  getAgents(departmentId?: number): Observable<UserLiteDto[]> {
    const params: Record<string, string> = {};
    if (departmentId) params['departmentId'] = String(departmentId);
    return this.http.get<UserLiteDto[]>(`${this.base}/agents`, { params });
  }
}
