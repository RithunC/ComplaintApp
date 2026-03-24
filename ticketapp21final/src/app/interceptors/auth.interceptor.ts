import { HttpInterceptorFn, HttpErrorResponse, HttpContext, HttpContextToken } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, finalize, throwError } from 'rxjs';
import { LoadingService } from '../services/loading.service';
import { ToastService } from '../services/toast.service';

/** Set this token to true on requests where 404 should be silently ignored */
export const SILENT_404 = new HttpContextToken<boolean>(() => false);

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const loading = inject(LoadingService);
  const toast   = inject(ToastService);
  const router  = inject(Router);

  const token = localStorage.getItem('jwt_token');
  const cloned = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  const isUnauthLookup = req.url.includes('/lookups') && !token;
  const isAuthCall     = req.url.includes('/authentication/');
  const silent404      = req.context.get(SILENT_404);

  if (!isUnauthLookup) loading.show();

  return next(cloned).pipe(
    catchError((err: HttpErrorResponse) => {
      const currentUrl = router.url ?? '';
      const onAuthPage = currentUrl.includes('/login') || currentUrl.includes('/register');

      if (isUnauthLookup) return throwError(() => err);
      if (isAuthCall)     return throwError(() => err);

      const extractMsg = (): string => {
        if (!err.error) return err.message || 'An error occurred.';
        if (typeof err.error === 'string') return err.error;
        if (err.error.message) return err.error.message;
        if (err.error.title)   return err.error.title;
        return 'An error occurred.';
      };

      switch (err.status) {
        case 401:
          if (!onAuthPage && token) {
            localStorage.removeItem('jwt_token');
            localStorage.removeItem('current_user');
            router.navigate(['/login']);
            toast.error('Session expired. Please login again.');
          }
          break;
        case 403:
          toast.error('Access denied. You do not have permission for this action.');
          break;
        case 404:
          // Suppress 404 toast when the caller explicitly opted out (e.g. ticket ID probing)
          if (!silent404) {
            toast.error('Resource not found.');
          }
          break;
        case 409: {
          const m409 = extractMsg();
          toast.warning(m409 || 'A conflict occurred.');
          break;
        }
        case 500: {
          const m500 = extractMsg();
          toast.error(`Server error: ${m500}`);
          break;
        }
        default:
          if (err.status !== 0) toast.error(extractMsg());
      }
      return throwError(() => err);
    }),
    finalize(() => { if (!isUnauthLookup) loading.hide(); })
  );
};
