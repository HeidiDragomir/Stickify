import { Routes } from '@angular/router';
import { AUTHENTICATION_PATHS, ROOT_PATHS } from '../../core/constants/paths.constants.js';
import { Login } from './pages/login/login.js';
import { Register } from './pages/register/register.js';

export const AUTHENTICATION_ROUTES: Routes = [
  {
    path: AUTHENTICATION_PATHS.login,
    component: Login,
    // canActivate: [noAuthenticationGuard],
  },
  {
    path: AUTHENTICATION_PATHS.register,
    component: Register,
    // canActivate: [noAuthenticationGuard],
  },
  { path: '**', redirectTo: ROOT_PATHS.notFound },
];
