import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { ReportService } from '../../../services/report.service';
import { AuthService } from '../../../services/auth.service';
import { TicketSummaryDto } from '../../../models';

@Component({
  selector: 'app-employee-dashboard',
  standalone: true,
  imports: [RouterLink, NavbarComponent],
  templateUrl: './employee-dashboard.component.html',
  styleUrls: ['./employee-dashboard.component.css']
})
export class EmployeeDashboardComponent implements OnInit {
  auth = inject(AuthService);
  rpt  = inject(ReportService);
  summary = signal<TicketSummaryDto | null>(null);
  ngOnInit(): void { this.rpt.getTicketSummary().subscribe(s => this.summary.set(s)); }
}
