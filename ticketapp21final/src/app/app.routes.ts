import { Routes } from '@angular/router';
import { adminGuard, agentGuard, employeeGuard, guestGuard, authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  { path: 'login',    canActivate: [guestGuard], loadComponent: () => import('./pages/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', canActivate: [guestGuard], loadComponent: () => import('./pages/auth/register/register.component').then(m => m.RegisterComponent) },

  { path: 'profile', canActivate: [authGuard], loadComponent: () => import('./pages/profile/profile.component').then(m => m.ProfileComponent) },

  {
    path: 'admin', canActivate: [adminGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard',   loadComponent: () => import('./pages/admin/dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent) },
      { path: 'tickets',     loadComponent: () => import('./pages/admin/tickets/admin-tickets.component').then(m => m.AdminTicketsComponent) },
      { path: 'tickets/:id', loadComponent: () => import('./pages/admin/ticket-detail/admin-ticket-detail.component').then(m => m.AdminTicketDetailComponent) },
      { path: 'users',       loadComponent: () => import('./pages/admin/users/admin-users.component').then(m => m.AdminUsersComponent) },
    ]
  },

  {
    path: 'agent', canActivate: [agentGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard',   loadComponent: () => import('./pages/agent/dashboard/agent-dashboard.component').then(m => m.AgentDashboardComponent) },
      { path: 'tickets',     loadComponent: () => import('./pages/agent/tickets/agent-tickets.component').then(m => m.AgentTicketsComponent) },
      { path: 'tickets/:id', loadComponent: () => import('./pages/agent/ticket-detail/agent-ticket-detail.component').then(m => m.AgentTicketDetailComponent) },
    ]
  },

  {
    path: 'employee', canActivate: [employeeGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard',     loadComponent: () => import('./pages/employee/dashboard/employee-dashboard.component').then(m => m.EmployeeDashboardComponent) },
      { path: 'create-ticket', loadComponent: () => import('./pages/employee/create-ticket/create-ticket.component').then(m => m.CreateTicketComponent) },
      { path: 'my-tickets',    loadComponent: () => import('./pages/employee/my-tickets/my-tickets.component').then(m => m.MyTicketsComponent) },
      { path: 'tickets/:id',   loadComponent: () => import('./pages/employee/ticket-detail/emp-ticket-detail.component').then(m => m.EmpTicketDetailComponent) },
    ]
  },

  { path: '**', redirectTo: 'login' }
];
