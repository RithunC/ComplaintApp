import { Component, inject } from '@angular/core';
import { LoadingService } from '../../../services/loading.service';
@Component({ selector: 'app-spinner', standalone: true, templateUrl: './spinner.component.html', styleUrls: ['./spinner.component.css'] })
export class SpinnerComponent { loading = inject(LoadingService); }
