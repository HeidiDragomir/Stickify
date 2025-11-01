import { Routes } from '@angular/router';
import { AUTHENTICATION_PATHS, ROOT_PATHS } from './core/constants/paths.constants.js';
import { Login } from './features/authentication/pages/login/login.js';
import { Register } from './features/authentication/pages/register/register.js';
import { Home } from './features/home/home.js';
import { MainLayout } from './core/layouts/main-layout/main-layout.js';

export const routes: Routes = [
    {
        path: '',
        component: MainLayout,
        children: [
            { path: ROOT_PATHS.base, component: Home },
            {
                path: AUTHENTICATION_PATHS.login,
                loadComponent: () =>
                    import('./features/authentication/pages/login/login').then((m) => m.Login),
            },
            {
                path: AUTHENTICATION_PATHS.register,
                loadComponent: () =>
                    import('./features/authentication/pages/register/register').then(
                        (m) => m.Register
                    ),
            },
        ],
    },
];
