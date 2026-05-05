import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

export type LoginRequest = {
  userNameOrEmail: string;
  password: string;
};

export type LoginResponse = {
  accessToken: string;
  tokenType: string;
  expiresInSeconds: number;
  userId: string;
  userName: string;
  email: string;
  role: string;
};

const TOKEN_KEY = 'access_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  async login(request: LoginRequest): Promise<LoginResponse> {
    try {
      const data = await firstValueFrom(
        this.http.post<LoginResponse>('/api/auth/login', request),
      );

      localStorage.setItem(TOKEN_KEY, data.accessToken);
      return data;
    } catch (err) {
      if (err instanceof HttpErrorResponse) {
        if (err.status === 401) {
          throw new Error('Invalid username/email or password.');
        }

        const bodyText =
          typeof err.error === 'string'
            ? err.error
            : err.error
              ? JSON.stringify(err.error)
              : '';

        throw new Error(
          `Login request failed (${err.status}). ${bodyText}`.trim(),
        );
      }

      throw err instanceof Error ? err : new Error('Login failed.');
    }
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }
}

