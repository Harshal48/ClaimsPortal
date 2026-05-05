import { CommonModule } from '@angular/common';
import { Component, inject, type OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import {
  ReviewService,
  type TaxReviewHistoryItem,
} from '../../services/review.service';
import type { TaxReturn } from '../../services/tax-return.service';

@Component({
  selector: 'app-review',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './review.html',
  styleUrl: './review.css',
})
export class ReviewComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly reviewApi = inject(ReviewService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  loadingList = false;
  listError: string | null = null;
  pending: TaxReturn[] = [];

  selected: TaxReturn | null = null;
  readonly commentCtrl = this.fb.nonNullable.control('');

  loadingHistory = false;
  historyError: string | null = null;
  history: TaxReviewHistoryItem[] = [];

  actionError: string | null = null;
  acting = false;

  async ngOnInit(): Promise<void> {
    await this.refreshPending();
  }

  async refreshPending(): Promise<void> {
    this.listError = null;
    this.loadingList = true;
    try {
      this.pending = await this.reviewApi.getPending();
      if (this.selected && !this.pending.some((x) => x.id === this.selected!.id)) {
        this.selected = null;
        this.history = [];
      }
    } catch (err) {
      this.listError =
        err instanceof Error ? err.message : 'Could not load pending reviews.';
    } finally {
      this.loadingList = false;
    }
  }

  async select(row: TaxReturn): Promise<void> {
    this.selected = row;
    this.commentCtrl.setValue('');
    this.actionError = null;
    await this.loadHistory(row.id);
  }

  async loadHistory(returnId: string): Promise<void> {
    this.historyError = null;
    this.loadingHistory = true;
    try {
      this.history = await this.reviewApi.getHistory(returnId);
    } catch (err) {
      this.history = [];
      this.historyError =
        err instanceof Error ? err.message : 'Could not load history.';
    } finally {
      this.loadingHistory = false;
    }
  }

  private payload(): { comments: string | null } {
    const c = this.commentCtrl.value.trim();
    return { comments: c.length ? c : null };
  }

  async approve(): Promise<void> {
    if (!this.selected) return;
    await this.runAction(() =>
      this.reviewApi.approve(this.selected!.id, this.payload()),
    );
  }

  async reject(): Promise<void> {
    if (!this.selected) return;
    await this.runAction(() =>
      this.reviewApi.reject(this.selected!.id, this.payload()),
    );
  }

  async requestChanges(): Promise<void> {
    if (!this.selected) return;
    await this.runAction(() =>
      this.reviewApi.requestChanges(this.selected!.id, this.payload()),
    );
  }

  private async runAction(fn: () => Promise<TaxReturn>): Promise<void> {
    this.actionError = null;
    this.acting = true;
    try {
      await fn();
      this.commentCtrl.setValue('');
      await this.refreshPending();
      if (this.selected) {
        await this.loadHistory(this.selected.id);
      }
    } catch (err) {
      this.actionError =
        err instanceof Error ? err.message : 'Action failed.';
    } finally {
      this.acting = false;
    }
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }
}
