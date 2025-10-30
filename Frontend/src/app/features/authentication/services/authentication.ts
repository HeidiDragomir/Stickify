import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { LoginRequest } from '../types/login-request.js';
import { map, Observable } from 'rxjs';
import { LoginResponse } from '../types/login-response.js';
import { getEndpoints } from '../../../core/constants/endpoints.constants.js';
import { RegisterRequest } from '../types/register-request.js';
import { RegisterResponse } from '../types/register-response.js';

@Injectable({
    providedIn: 'root',
})
export class Authentication {
    private readonly endpoints = getEndpoints();

    http = inject(HttpClient);

    register(credentials: RegisterRequest): Observable<RegisterResponse> {
        return this.http.post<any>(this.endpoints.auth.register, credentials).pipe(
            map((response) => ({
                user: {
                    id: response.userId,
                    userName: response.userName,
                    email: response.email,
                    role: response.role,
                },
                accessToken: response.token,
                expireAt: response.expireAt,
            }))
        );
    }

    login(credentials: LoginRequest): Observable<LoginResponse> {
        return this.http.post<any>(this.endpoints.auth.login, credentials).pipe(
            map((response) => ({
                user: {
                    id: response.userId,
                    userName: response.userName,
                    email: response.email,
                    role: response.role,
                },
                accessToken: response.token,
                expireAt: response.expireAt,
            }))
        );
    }
}
