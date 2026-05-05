import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

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
    loadComponent: () =>
      import('./pages/return-detail/return-detail').then(
        (m) => m.ReturnDetailComponent,
      ),
  },
  {
    path: 'returns',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/returns/returns').then((m) => m.ReturnsComponent),
  },
];
