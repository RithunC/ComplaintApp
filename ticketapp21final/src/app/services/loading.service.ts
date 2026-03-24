import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private _n = 0;
  loading = signal(false);
  show(): void { this._n++; this.loading.set(true); }
  hide(): void { this._n = Math.max(0, this._n - 1); if (this._n === 0) this.loading.set(false); }
}
