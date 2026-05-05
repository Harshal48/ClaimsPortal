import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { ReturnDetailComponent } from './pages/return-detail/return-detail';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login',
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login/login').then((m) => m.LoginComponent),
  },
  {
    path: 'home',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/home/home').then((m) => m.HomeComponent),
  },
  {
    path: 'taxpayers',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/taxpayers/taxpayers').then((m) => m.TaxpayersComponent),
  },
  {
    path: 'returns/:id',
    canActivate: [authGuard],
    // Eager standalone route: avoids lazy-chunk fetch on navigation (often the
    // main "slow" feeling in dev, especially over OneDrive / cold cache).
    component: ReturnDetailComponent,
  },
  {
    path: 'returns',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/returns/returns').then((m) => m.ReturnsComponent),
  },
  {
    path: 'review',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/review/review').then((m) => m.ReviewComponent),
  },
];
