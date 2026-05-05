import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

export type TaxReturn = {
  id: string;
  taxpayerId: string;
  taxYear: number;
  returnStatus: string;
  filingStatus: string;
  reviewerId?: string | null;
  totalIncome: number;
  totalWithheld: number;
  totalDeduction: number;
  createdAtUtc: string;
  updatedAtUtc: string;
};

export type CreateTaxReturnRequest = {
  taxpayerId: string;
  taxYear: number;
  filingStatus: string;
};

@Injectable({ providedIn: 'root' })
export class TaxReturnService {
  private readonly http = inject(HttpClient);

  async getAll(): Promise<TaxReturn[]> {
    try {
      return await firstValueFrom(
        this.http.get<TaxReturn[]>('/api/returns'),
      );
    } catch (err) {
      throw this.toError(err, 'Could not load tax returns.');
    }
  }

  async create(request: CreateTaxReturnRequest): Promise<TaxReturn> {
    try {
      return await firstValueFrom(
        this.http.post<TaxReturn>('/api/returns', request),
      );
    } catch (err) {
      throw this.toError(err, 'Could not create tax return.');
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
