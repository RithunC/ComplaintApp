import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CommentCreateDto, CommentResponseDto, CommentEditDto } from '../models';

@Injectable({ providedIn: 'root' })
export class CommentService {
  private base = `${environment.apiUrl}/comments`;
  constructor(private http: HttpClient) {}
  add(dto: CommentCreateDto):            Observable<CommentResponseDto>   { return this.http.post<CommentResponseDto>(this.base, dto); }
  getByTicket(id: number):               Observable<CommentResponseDto[]> { return this.http.get<CommentResponseDto[]>(`${this.base}/ticket/${id}`); }
  edit(id: number, dto: CommentEditDto): Observable<CommentResponseDto>   { return this.http.patch<CommentResponseDto>(`${this.base}/${id}`, dto); }
  delete(id: number):                    Observable<void>                  { return this.http.delete<void>(`${this.base}/${id}`); }
}
