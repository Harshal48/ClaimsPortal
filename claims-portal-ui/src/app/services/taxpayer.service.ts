import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

export type Taxpayer = {
  id: string;
  taxpayerNumber: string;
  legalName: string;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
  createdByUserId?: string | null;
};

export type CreateTaxpayerRequest = {
  taxpayerNumber: string;
  legalName: string;
  createdByUserId?: string | null;
};

@Injectable({ providedIn: 'root' })
export class TaxpayerService {
  private readonly http = inject(HttpClient);

  async getAll(): Promise<Taxpayer[]> {
    try {
      return await firstValueFrom(
        this.http.get<Taxpayer[]>('/api/taxpayers'),
      );
    } catch (err) {
      throw this.toError(err, 'Could not load taxpayers.');
    }
  }

  async getById(id: string): Promise<Taxpayer> {
    try {
      return await firstValueFrom(
        this.http.get<Taxpayer>(
          `/api/taxpayers/${encodeURIComponent(id)}`,
        ),
      );
    } catch (err) {
      throw this.toError(err, 'Could not load taxpayer.');
    }
  }

  async create(request: CreateTaxpayerRequest): Promise<Taxpayer> {
    try {
      return await firstValueFrom(
        this.http.post<Taxpayer>('/api/taxpayers', request),
      );
    } catch (err) {
      throw this.toError(err, 'Could not create taxpayer.');
    }
  }

  private toError(err: unknown, fallback: string): Error {
    if (err instanceof HttpErrorResponse) {
      if (err.status === 401) {
        return new Error('You are not signed in, or your session expired.');
      }
      if (err.status === 403) {
        return new Error('You do not have permission for this action.');
      }
      const bodyText =
        typeof err.error === 'string'
          ? err.error
          : err.error
            ? JSON.stringify(err.error)
            : '';
      return new Error(`${fallback} (${err.status}). ${bodyText}`.trim());
    }
    return err instanceof Error ? err : new Error(fallback);
  }
}
