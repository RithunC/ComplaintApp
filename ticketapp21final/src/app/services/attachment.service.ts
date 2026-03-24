import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AttachmentResponseDto } from '../models';

@Injectable({ providedIn: 'root' })
export class AttachmentService {
  private base = `${environment.apiUrl}/attachments`;
  constructor(private http: HttpClient) {}
  upload(ticketId: number, file: File): Observable<AttachmentResponseDto> {
    const fd = new FormData(); fd.append('file', file);
    return this.http.post<AttachmentResponseDto>(`${this.base}/${ticketId}`, fd);
  }
  getByTicket(id: number): Observable<AttachmentResponseDto[]> { return this.http.get<AttachmentResponseDto[]>(`${this.base}/${id}`); }
  download(id: number):    Observable<Blob>                    { return this.http.get(`${this.base}/${id}/download`, { responseType: 'blob' }); }
  delete(id: number):      Observable<void>                    { return this.http.delete<void>(`${this.base}/${id}`); }
}
