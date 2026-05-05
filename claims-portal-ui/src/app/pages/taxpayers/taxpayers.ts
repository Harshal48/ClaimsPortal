import { CommonModule } from '@angular/common';
import { Component, inject, type OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import {
  TaxpayerService,
  type Taxpayer,
} from '../../services/taxpayer.service';

@Component({
  selector: 'app-taxpayers',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './taxpayers.html',
  styleUrl: './taxpayers.css',
})
export class TaxpayersComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly taxpayersApi = inject(TaxpayerService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  loadingList = false;
  saving = false;
  listError: string | null = null;
  formError: string | null = null;
  taxpayers: Taxpayer[] = [];

  readonly form = this.fb.nonNullable.group({
    taxpayerNumber: ['', [Validators.required]],
    legalName: ['', [Validators.required]],
  });

  async ngOnInit(): Promise<void> {
    await this.refreshList();
  }

  async refreshList(): Promise<void> {
    this.listError = null;
    this.loadingList = true;
    try {
      this.taxpayers = await this.taxpayersApi.getAll();
    } catch (err) {
      this.listError =
        err instanceof Error ? err.message : 'Could not load taxpayers.';
    } finally {
      this.loadingList = false;
    }
  }

  async onCreate(): Promise<void> {
    this.formError = null;
    if (this.form.invalid) return;

    this.saving = true;
    try {
      const { taxpayerNumber, legalName } = this.form.getRawValue();
      await this.taxpayersApi.create({
        taxpayerNumber: taxpayerNumber.trim(),
        legalName: legalName.trim(),
      });
      this.form.reset();
      await this.refreshList();
    } catch (err) {
      this.formError =
        err instanceof Error ? err.message : 'Could not create taxpayer.';
    } finally {
      this.saving = false;
    }
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }
}
