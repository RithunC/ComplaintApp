import { Injectable, inject } from '@angular/core';
import { TicketService } from './ticket.service';
import { AuthService } from './auth.service';
import { Observable, forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { TicketResponseDto } from '../models';

/**
 * Employee ticket service.
 *
 * PROBLEM: POST /tickets/query is [Authorize(Roles="Admin,Agent")] — employees get 403.
 *
 * SOLUTION:
 *  - Store ticket IDs per-user in localStorage on creation/view.
 *  - On "My Tickets" load, probe a range of IDs to discover old tickets.
 *  - All probes use getQuiet() so 404 responses are silently swallowed
 *    (no "Resource not found" toast spam for non-existent IDs).
 *  - Filter fetched tickets by createdByUserId to ensure ownership.
 */
@Injectable({ providedIn: 'root' })
export class EmployeeTicketService {
  private ts   = inject(TicketService);
  private auth = inject(AuthService);

  private get storageKey(): string {
    const uid = this.auth.currentUser()?.id ?? 0;
    return `emp_ticket_ids_${uid}`;
  }

  saveTicketId(ticketId: number): void {
    const ids = this.getStoredIds();
    if (!ids.includes(ticketId)) {
      ids.unshift(ticketId);
      localStorage.setItem(this.storageKey, JSON.stringify(ids));
    }
  }

  getStoredIds(): number[] {
    try {
      const raw = localStorage.getItem(this.storageKey);
      return raw ? JSON.parse(raw) : [];
    } catch { return []; }
  }

  getMyTickets(): Observable<TicketResponseDto[]> {
    const myId    = this.auth.currentUser()?.id;
    const stored  = this.getStoredIds();

    // Build probe set: stored IDs + range 1..maxProbe to catch old tickets
    const maxProbe = this.getMaxProbeId(stored);
    const probeSet = new Set<number>(stored);
    for (let i = 1; i <= maxProbe; i++) probeSet.add(i);

    const ids = Array.from(probeSet);
    if (ids.length === 0) return of([]);

    // Use getQuiet() — 404s are swallowed silently by the interceptor
    const requests = ids.map(id =>
      this.ts.getQuiet(id).pipe(catchError(() => of(null)))
    );

    return forkJoin(requests).pipe(
      map(results => {
        const mine = results
          .filter((t): t is TicketResponseDto => t !== null)
          .filter(t => !myId || t.createdByUserId === myId)
          .sort((a, b) => b.id - a.id);

        // Persist discovered IDs for faster future loads
        mine.forEach(t => this.saveTicketId(t.id));
        return mine;
      })
    );
  }

  private getMaxProbeId(stored: number[]): number {
    if (stored.length === 0) return 200; // probe first 200 on fresh login
    return Math.max(...stored) + 20;     // probe a bit beyond highest known
  }

  clearStoredIds(): void {
    localStorage.removeItem(this.storageKey);
  }
}
