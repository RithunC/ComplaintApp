import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';
import { environment } from '../../environments/environment';
import { DepartmentDto, RoleDto, CategoryDto, PriorityDto, StatusDto } from '../models';

@Injectable({ providedIn: 'root' })
export class LookupService {
  private base = `${environment.apiUrl}/lookups`;
  private _d$?: Observable<DepartmentDto[]>;
  private _r$?: Observable<RoleDto[]>;
  private _c$?: Observable<CategoryDto[]>;
  private _p$?: Observable<PriorityDto[]>;
  private _s$?: Observable<StatusDto[]>;

  constructor(private http: HttpClient) {}

  getDepartments(): Observable<DepartmentDto[]> {
    return this._d$ ??= this.http.get<DepartmentDto[]>(`${this.base}/departments`).pipe(shareReplay(1));
  }
  getRoles(): Observable<RoleDto[]> {
    return this._r$ ??= this.http.get<RoleDto[]>(`${this.base}/roles`).pipe(shareReplay(1));
  }
  getCategories(): Observable<CategoryDto[]> {
    return this._c$ ??= this.http.get<CategoryDto[]>(`${this.base}/categories`).pipe(shareReplay(1));
  }
  getPriorities(): Observable<PriorityDto[]> {
    return this._p$ ??= this.http.get<PriorityDto[]>(`${this.base}/priorities`).pipe(shareReplay(1));
  }
  getStatuses(): Observable<StatusDto[]> {
    return this._s$ ??= this.http.get<StatusDto[]>(`${this.base}/statuses`).pipe(shareReplay(1));
  }

  /** Call after login so authenticated endpoints get fresh data */
  clearCache(): void {
    this._d$ = undefined;
    this._r$ = undefined;
    this._c$ = undefined;
    this._p$ = undefined;
    this._s$ = undefined;
  }
}
