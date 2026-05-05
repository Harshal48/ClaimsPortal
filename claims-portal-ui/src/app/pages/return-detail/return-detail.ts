import { CommonModule } from '@angular/common';
import { Component, inject, type OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
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
  selector: 'app-return-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './return-detail.html',
  styleUrl: './return-detail.css',
})
export class ReturnDetailComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly returnsApi = inject(TaxReturnService);
  private readonly taxpayersApi = inject(TaxpayerService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  returnId: string | null = null;
  loading = false;
  /** Loaded lazily via GET /api/taxpayers/:id for the header label only. */
  headerTaxpayer: Taxpayer | null = null;
  ret: TaxReturn | null = null;

  loadError: string | null = null;
  basicsError: string | null = null;
  incomeError: string | null = null;
  deductionError: string | null = null;
  submitError: string | null = null;

  savingBasics = false;
  savingIncome = false;
  savingDeduction = false;
  submitting = false;

  readonly filingOptions = [
    'Single',
    'MarriedFilingJointly',
    'MarriedFilingSeparately',
    'HeadOfHousehold',
  ] as const;

  readonly basicsForm = this.fb.nonNullable.group({
    taxYear: [
      new Date().getFullYear(),
      [Validators.required, Validators.min(1900), Validators.max(2100)],
    ],
    filingStatus: ['Single', [Validators.required]],
  });

  readonly incomeForm = this.fb.nonNullable.group({
    amount: [1, [Validators.required, Validators.min(0.01)]],
  });

  readonly deductionForm = this.fb.nonNullable.group({
    deductionName: ['', [Validators.required]],
    deductionType: ['', [Validators.required]],
    amount: [1, [Validators.required, Validators.min(0.01)]],
  });

  async ngOnInit(): Promise<void> {
    this.returnId = this.route.snapshot.paramMap.get('id');
    if (!this.returnId) {
      this.loadError = 'Missing return id in URL.';
      return;
    }

    await this.reloadReturn();
  }

  get isDraft(): boolean {
    const s = this.ret?.returnStatus ?? '';
    return s.toLowerCase() === 'draft';
  }

  taxpayerLabel(id: string): string {
    const t = this.headerTaxpayer;
    if (t?.id === id) {
      return `${t.taxpayerNumber} — ${t.legalName}`;
    }
    return id.slice(0, 8) + '…';
  }

  private async loadHeaderTaxpayer(taxpayerId: string): Promise<void> {
    try {
      const t = await this.taxpayersApi.getById(taxpayerId);
      if (this.ret?.taxpayerId === taxpayerId) {
        this.headerTaxpayer = t;
      }
    } catch {
      // Non-fatal; header falls back to truncated id via taxpayerLabel.
    }
  }

  private applyFormsFromReturn(r: TaxReturn): void {
    this.basicsForm.patchValue({
      taxYear: r.taxYear,
      filingStatus: r.filingStatus,
    });
  }

  async reloadReturn(): Promise<void> {
    if (!this.returnId) return;

    this.loadError = null;
    this.loading = true;
    try {
      const r = await this.returnsApi.getById(this.returnId);
      this.ret = r;
      this.applyFormsFromReturn(r);
    } catch (err) {
      this.ret = null;
      this.loadError =
        err instanceof Error ? err.message : 'Could not load tax return.';
    } finally {
      this.loading = false;
    }
  }

  async onSaveBasics(): Promise<void> {
    if (!this.returnId || !this.isDraft) return;
    this.basicsError = null;
    if (this.basicsForm.invalid) return;

    this.savingBasics = true;
    try {
      const raw = this.basicsForm.getRawValue();
      this.ret = await this.returnsApi.update(this.returnId, {
        taxYear: Number(raw.taxYear),
        filingStatus: raw.filingStatus.trim(),
      });
      this.applyFormsFromReturn(this.ret);
    } catch (err) {
      this.basicsError =
        err instanceof Error ? err.message : 'Could not update return.';
    } finally {
      this.savingBasics = false;
    }
  }

  async onAddIncome(): Promise<void> {
    if (!this.returnId || !this.isDraft) return;
    this.incomeError = null;
    if (this.incomeForm.invalid) return;

    this.savingIncome = true;
    try {
      const { amount } = this.incomeForm.getRawValue();
      this.ret = await this.returnsApi.addIncome(this.returnId, {
        amount: Number(amount),
      });
      this.incomeForm.reset({ amount: 1 });
    } catch (err) {
      this.incomeError =
        err instanceof Error ? err.message : 'Could not add income.';
    } finally {
      this.savingIncome = false;
    }
  }

  async onAddDeduction(): Promise<void> {
    if (!this.returnId || !this.isDraft) return;
    this.deductionError = null;
    if (this.deductionForm.invalid) return;

    this.savingDeduction = true;
    try {
      const raw = this.deductionForm.getRawValue();
      this.ret = await this.returnsApi.addDeduction(this.returnId, {
        deductionName: raw.deductionName.trim(),
        deductionType: raw.deductionType.trim(),
        amount: Number(raw.amount),
      });
      this.deductionForm.reset({
        deductionName: '',
        deductionType: '',
        amount: 1,
      });
    } catch (err) {
      this.deductionError =
        err instanceof Error ? err.message : 'Could not add deduction.';
    } finally {
      this.savingDeduction = false;
    }
  }

  async onSubmit(): Promise<void> {
    if (!this.returnId || !this.isDraft) return;
    this.submitError = null;

    this.submitting = true;
    try {
      this.ret = await this.returnsApi.submit(this.returnId);
    } catch (err) {
      this.submitError =
        err instanceof Error ? err.message : 'Could not submit return.';
    } finally {
      this.submitting = false;
    }
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }
}
