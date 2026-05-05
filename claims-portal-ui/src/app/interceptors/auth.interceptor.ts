import { HttpInterceptorFn } from '@angular/common/http';

const TOKEN_KEY = 'access_token';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const url = req.url;

  // Login must stay unauthenticated.
  if (url.startsWith('/api/auth/login')) {
    return next(req);
  }

  // Only attach bearer tokens for our API calls.
  if (!url.startsWith('/api')) {
    return next(req);
  }

  const token = localStorage.getItem(TOKEN_KEY);
  if (!token) {
    return next(req);
  }

  return next(
    req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    }),
  );
};
