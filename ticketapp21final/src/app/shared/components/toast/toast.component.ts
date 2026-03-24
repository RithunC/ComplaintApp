import { Component, inject } from '@angular/core';
import { ToastService } from '../../../services/toast.service';
@Component({ selector: 'app-toast', standalone: true, templateUrl: './toast.component.html', styleUrls: ['./toast.component.css'] })
export class ToastComponent {
  svc = inject(ToastService);
  icon(t: string): string {
    return ({success:'bi-check-circle-fill', error:'bi-x-circle-fill', warning:'bi-exclamation-triangle-fill', info:'bi-info-circle-fill'} as Record<string,string>)[t] ?? 'bi-info-circle-fill';
  }
}
