import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { LoginRequest } from '../types/login-request.js';
import { map, Observable } from 'rxjs';
import { LoginResponse } from '../types/login-response.js';

@Injectable({
  providedIn: 'root',
})
export class Authentication {
  private readonly apiUrl = 'https://localhost:7251/api/auth/login';

  http = inject(HttpClient);

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<any>(this.apiUrl, credentials).pipe(
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
