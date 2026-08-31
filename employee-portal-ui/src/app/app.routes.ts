import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  // -------------------------
  // Public
  // -------------------------
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then((m) => m.Login),
  },

  // -------------------------
  // Protected application
  // -------------------------
  {
    path: 'employees',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/app-shell/app-shell').then((m) => m.AppShell),
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/employees/dashboard/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'add',
        loadComponent: () => import('./features/employees/add/add').then((m) => m.Add),
      },
      // {
      //   path: 'upload',
      //   loadComponent: () => import('./features/employees/upload/upload').then((m) => m.Upload),
      // },
    ],
  },

  // -------------------------
  // Root
  // -------------------------
  {
    path: '',
    redirectTo: 'employees',
    pathMatch: 'full',
  },

  // -------------------------
  // Unknown URL
  // -------------------------
  {
    path: '**',
    redirectTo: 'employees',
  },
];
