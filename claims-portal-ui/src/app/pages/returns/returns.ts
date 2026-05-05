import { CommonModule } from '@angular/common';
import { Component, inject, type OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import {
  TaxReturnService,
  type TaxReturn,
} from '../../services/tax-return.service';
import {
  TaxpayerService,
  type Taxpayer,
} from '../../services/taxpayer.service';

@Component({
  selector: 'app-returns',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './returns.html',
  styleUrl: './returns.css',
})
export class ReturnsComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly returnsApi = inject(TaxReturnService);
  private readonly taxpayersApi = inject(TaxpayerService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  loadingList = false;
  loadingTaxpayers = false;
  saving = false;
  listError: string | null = null;
  taxpayersError: string | null = null;
  formError: string | null = null;
  returns: TaxReturn[] = [];
  taxpayers: Taxpayer[] = [];

  readonly filingOptions = [
    'Single',
    'MarriedFilingJointly',
    'MarriedFilingSeparately',
    'HeadOfHousehold',
  ] as const;

  readonly form = this.fb.nonNullable.group({
    taxpayerId: ['', [Validators.required]],
    taxYear: [
      new Date().getFullYear(),
      [Validators.required, Validators.min(1900), Validators.max(2100)],
    ],
    filingStatus: ['Single', [Validators.required]],
  });

  async ngOnInit(): Promise<void> {
    await Promise.all([this.loadTaxpayers(), this.refreshList()]);
  }

  taxpayerLabel(id: string): string {
    const t = this.taxpayers.find((x) => x.id === id);
    if (!t) return id.slice(0, 8) + '…';
    return `${t.taxpayerNumber} — ${t.legalName}`;
  }

  async loadTaxpayers(): Promise<void> {
    this.taxpayersError = null;
    this.loadingTaxpayers = true;
    try {
      this.taxpayers = await this.taxpayersApi.getAll();
    } catch (err) {
      this.taxpayersError =
        err instanceof Error ? err.message : 'Could not load taxpayers.';
    } finally {
      this.loadingTaxpayers = false;
    }
  }

  async refreshList(): Promise<void> {
    this.listError = null;
    this.loadingList = true;
    try {
      this.returns = await this.returnsApi.getAll();
    } catch (err) {
      this.listError =
        err instanceof Error ? err.message : 'Could not load tax returns.';
    } finally {
      this.loadingList = false;
    }
  }

  async onCreate(): Promise<void> {
    this.formError = null;
    if (this.form.invalid) return;

    this.saving = true;
    try {
      const raw = this.form.getRawValue();
      await this.returnsApi.create({
        taxpayerId: raw.taxpayerId,
        taxYear: Number(raw.taxYear),
        filingStatus: raw.filingStatus.trim(),
      });
      await this.refreshList();
    } catch (err) {
      this.formError =
        err instanceof Error ? err.message : 'Could not create tax return.';
    } finally {
      this.saving = false;
    }
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }
}
