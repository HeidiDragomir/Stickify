import { Routes } from '@angular/router';
import { AUTHENTICATION_PATHS, ROOT_PATHS } from './core/constants/paths.constants.js';

export const routes: Routes = [
  // {
  //     path: ROOT_PATHS.home,
  //     component: HomeComponent
  // },
  {
    path: AUTHENTICATION_PATHS.base,
    children: [
      {
        path: AUTHENTICATION_PATHS.login,
        loadComponent: () =>
          import('./features/authentication/pages/login/login').then((m) => m.Login),
      },
      {
        path: AUTHENTICATION_PATHS.register,
        loadComponent: () =>
          import('./features/authentication/pages/register/register').then((m) => m.Register),
      },
    ],
  },
  //   { path: '404', component: Error404Component },
  { path: '**', redirectTo: '404' },
];
