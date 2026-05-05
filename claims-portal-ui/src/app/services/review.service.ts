import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import type { TaxReturn } from './tax-return.service';

export type TaxReviewHistoryItem = {
  id: string;
  taxReturnId: string;
  oldStatus: string;
  newStatus: string;
  reviewerId: string;
  comments?: string | null;
  createdAtUtc: string;
};

export type ReviewActionRequest = {
  comments?: string | null;
};

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private readonly http = inject(HttpClient);

  async getPending(): Promise<TaxReturn[]> {
    try {
      return await firstValueFrom(
        this.http.get<TaxReturn[]>('/api/review/pending'),
      );
    } catch (err) {
      throw this.toError(err, 'Could not load pending reviews.');
    }
  }

  async getHistory(returnId: string): Promise<TaxReviewHistoryItem[]> {
    try {
      return await firstValueFrom(
        this.http.get<TaxReviewHistoryItem[]>(
          `/api/review/${encodeURIComponent(returnId)}`,
        ),
      );
    } catch (err) {
      throw this.toError(err, 'Could not load review history.');
    }
  }

  async approve(
    returnId: string,
    request: ReviewActionRequest,
  ): Promise<TaxReturn> {
    try {
      return await firstValueFrom(
        this.http.post<TaxReturn>(
          `/api/review/${encodeURIComponent(returnId)}/approve`,
          request,
        ),
      );
    } catch (err) {
      throw this.toError(err, 'Could not approve return.');
    }
  }

  async reject(
    returnId: string,
    request: ReviewActionRequest,
  ): Promise<TaxReturn> {
    try {
      return await firstValueFrom(
        this.http.post<TaxReturn>(
          `/api/review/${encodeURIComponent(returnId)}/reject`,
          request,
        ),
      );
    } catch (err) {
      throw this.toError(err, 'Could not reject return.');
    }
  }

  async requestChanges(
    returnId: string,
    request: ReviewActionRequest,
  ): Promise<TaxReturn> {
    try {
      return await firstValueFrom(
        this.http.post<TaxReturn>(
          `/api/review/${encodeURIComponent(returnId)}/request-changes`,
          request,
        ),
      );
    } catch (err) {
      throw this.toError(err, 'Could not request changes.');
    }
  }

  private toError(err: unknown, fallback: string): Error {
    if (err instanceof HttpErrorResponse) {
      if (err.status === 401) {
        return new Error('You are not signed in, or your session expired.');
      }
      if (err.status === 403) {
        return new Error('You do not have permission to use review (Admin or Reviewer only).');
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
