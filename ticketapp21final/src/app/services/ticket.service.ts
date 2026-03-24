import { Injectable } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { SILENT_404 } from '../interceptors/auth.interceptor';
import {
  TicketCreateDto, TicketUpdateDto, TicketQueryDto,
  TicketResponseDto, TicketListItemDto, PagedResult,
  TicketStatusUpdateDto, TicketAssignRequestDto,
  TicketAutoAssignRequestDto, TicketAssignmentResponseDto,
  TicketStatusHistoryDto
} from '../models';

@Injectable({ providedIn: 'root' })
export class TicketService {
  private base = `${environment.apiUrl}/tickets`;
  constructor(private http: HttpClient) {}

  create(dto: TicketCreateDto): Observable<TicketResponseDto> {
    return this.http.post<TicketResponseDto>(this.base, dto);
  }

  /** Standard GET — shows 404 toast if not found */
  get(id: number): Observable<TicketResponseDto> {
    return this.http.get<TicketResponseDto>(`${this.base}/${id}`);
  }

  /** Silent GET — suppresses 404 toast; used when probing unknown IDs */
  getQuiet(id: number): Observable<TicketResponseDto> {
    return this.http.get<TicketResponseDto>(
      `${this.base}/${id}`,
      { context: new HttpContext().set(SILENT_404, true) }
    );
  }

  query(dto: TicketQueryDto): Observable<PagedResult<TicketListItemDto>> {
    return this.http.post<PagedResult<TicketListItemDto>>(`${this.base}/query`, dto);
  }

  update(id: number, dto: TicketUpdateDto): Observable<TicketResponseDto> {
    return this.http.patch<TicketResponseDto>(`${this.base}/${id}`, dto);
  }

  updateStatus(id: number, dto: TicketStatusUpdateDto): Observable<void> {
    return this.http.post<void>(`${this.base}/${id}/status`, dto);
  }

  assign(id: number, dto: TicketAssignRequestDto): Observable<TicketAssignmentResponseDto> {
    return this.http.post<TicketAssignmentResponseDto>(`${this.base}/${id}/assign`, dto);
  }

  autoAssign(id: number, dto: TicketAutoAssignRequestDto): Observable<TicketAssignmentResponseDto> {
    return this.http.post<TicketAssignmentResponseDto>(`${this.base}/${id}/auto-assign`, dto);
  }

  getHistory(id: number): Observable<TicketStatusHistoryDto[]> {
    return this.http.get<TicketStatusHistoryDto[]>(`${this.base}/${id}/history`);
  }
}
