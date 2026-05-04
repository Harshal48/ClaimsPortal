import { Injectable } from '@angular/core';

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
  async login(request: LoginRequest): Promise<LoginResponse> {
    const res = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    });

    if (!res.ok) {
      throw new Error('Login failed');
    }

    const data = (await res.json()) as LoginResponse;
    localStorage.setItem(TOKEN_KEY, data.accessToken);
    return data;
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }
}

