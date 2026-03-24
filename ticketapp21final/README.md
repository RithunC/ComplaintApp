# TicketDesk — Angular 21 Frontend

Complete complaint ticket management system built with **Angular 21**.
Every component has **4 separate files**: `.ts` · `.html` · `.css` · `.spec.ts`

---

## 🚀 Quick Start

```bash
# Prerequisites: Node 20+, Angular CLI 21
npm install -g @angular/cli@21

# Install & run
cd ticketdesk
npm install
ng serve
# → http://localhost:4200
```

**Backend must be running on** `http://localhost:5251`

---

## 📁 Structure

```
src/app/
├── guards/          auth.guard.ts (authGuard, adminGuard, agentGuard, employeeGuard, guestGuard)
├── interceptors/    auth.interceptor.ts (JWT + loading + error handling)
├── models/          index.ts  (all DTOs matching backend exactly)
├── services/        auth · loading · toast · lookup · ticket · comment
│                    attachment · report · user · error-log
├── shared/
│   ├── components/
│   │   ├── navbar/   navbar.component.ts/html/css/spec.ts
│   │   ├── spinner/  spinner.component.ts/html/css/spec.ts
│   │   └── toast/    toast.component.ts/html/css/spec.ts
│   └── pipes/        replace.pipe.ts
└── pages/
    ├── auth/         login/  register/
    ├── admin/        dashboard/ tickets/ ticket-detail/ users/ error-logs/
    ├── agent/        dashboard/ tickets/ ticket-detail/
    └── employee/     dashboard/ create-ticket/ my-tickets/ ticket-detail/
```
Each page folder = `component.ts` + `component.html` + `component.css` + `component.spec.ts`

---

## 🔐 Role Routing

| Role | Login → | Pages |
|---|---|---|
| Admin | `/admin/dashboard` | Dashboard, All Tickets, Users, Error Logs |
| Agent | `/agent/dashboard` | Dashboard, My Queue |
| Employee | `/employee/dashboard` | Dashboard, Create Ticket, My Tickets |

---

## ✅ Angular 21 Features

- Standalone components only (no NgModules)
- `signal()` / `computed()` for all reactive state
- `@if` / `@for` new control flow (no *ngIf / *ngFor)
- `inject()` for dependency injection
- `HttpInterceptorFn` (functional interceptor)
- `CanActivateFn` (functional guards)
- `loadComponent()` dynamic routing
- `provideZoneChangeDetection` in app.config
- `shareReplay(1)` for lookup data caching
