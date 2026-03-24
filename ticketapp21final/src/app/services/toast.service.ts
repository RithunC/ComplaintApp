import { Injectable, signal } from '@angular/core';
export type ToastType = 'success' | 'error' | 'warning' | 'info';
export interface Toast { id: number; message: string; type: ToastType; }

@Injectable({ providedIn: 'root' })
export class ToastService {
  toasts = signal<Toast[]>([]);
  private _id = 0;
  show(message: string, type: ToastType = 'info', ms = 4000): void {
    const id = this._id++;
    this.toasts.update(t => [...t, { id, message, type }]);
    setTimeout(() => this.remove(id), ms);
  }
  success(m: string): void { this.show(m, 'success'); }
  error(m: string):   void { this.show(m, 'error'); }
  warning(m: string): void { this.show(m, 'warning'); }
  info(m: string):    void { this.show(m, 'info'); }
  remove(id: number): void { this.toasts.update(t => t.filter(x => x.id !== id)); }
}
