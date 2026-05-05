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

export type UpdateTaxReturnRequest = {
  taxYear: number;
  filingStatus: string;
};

export type AddIncomeRequest = {
  amount: number;
};

export type AddDeductionRequest = {
  deductionName: string;
  deductionType: string;
  amount: number;
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

  async getById(id: string): Promise<TaxReturn> {
    try {
      return await firstValueFrom(
        this.http.get<TaxReturn>(`/api/returns/${encodeURIComponent(id)}`),
      );
    } catch (err) {
      throw this.toError(err, 'Could not load tax return.');
    }
  }

  async update(id: string, request: UpdateTaxReturnRequest): Promise<TaxReturn> {
    try {
      return await firstValueFrom(
        this.http.put<TaxReturn>(
          `/api/returns/${encodeURIComponent(id)}`,
          request,
        ),
      );
    } catch (err) {
      throw this.toError(err, 'Could not update tax return.');
    }
  }

  async addIncome(id: string, request: AddIncomeRequest): Promise<TaxReturn> {
    try {
      return await firstValueFrom(
        this.http.post<TaxReturn>(
          `/api/returns/${encodeURIComponent(id)}/income`,
          request,
        ),
      );
    } catch (err) {
      throw this.toError(err, 'Could not add income.');
    }
  }

  async addDeduction(
    id: string,
    request: AddDeductionRequest,
  ): Promise<TaxReturn> {
    try {
      return await firstValueFrom(
        this.http.post<TaxReturn>(
          `/api/returns/${encodeURIComponent(id)}/deductions`,
          request,
        ),
      );
    } catch (err) {
      throw this.toError(err, 'Could not add deduction.');
    }
  }

  async submit(id: string): Promise<TaxReturn> {
    try {
      return await firstValueFrom(
        this.http.post<TaxReturn>(
          `/api/returns/${encodeURIComponent(id)}/submit`,
          {},
        ),
      );
    } catch (err) {
      throw this.toError(err, 'Could not submit tax return.');
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
