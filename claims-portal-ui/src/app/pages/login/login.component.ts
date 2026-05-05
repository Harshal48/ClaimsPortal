import { CommonModule } from '@angular/common';
import { Component, inject, type OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.css',
})
export class LoginComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  loading = false;
  error: string | null = null;

  readonly form = this.fb.nonNullable.group({
    userNameOrEmail: ['', [Validators.required]],
    password: ['', [Validators.required]],
  });

  ngOnInit(): void {
    if (this.auth.getAccessToken()) {
      void this.router.navigateByUrl('/home');
    }
  }

  async onSubmit(): Promise<void> {
    this.error = null;
    if (this.form.invalid) return;

    this.loading = true;
    try {
      const { userNameOrEmail, password } = this.form.getRawValue();
      await this.auth.login({ userNameOrEmail, password });

      const navigated = await this.router.navigateByUrl('/home');
      if (!navigated) {
        throw new Error('Navigation failed after login (no matching route).');
      }
    } catch (err) {
      this.error =
        err instanceof Error
          ? err.message
          : 'Invalid username/email or password.';
    } finally {
      this.loading = false;
    }
  }
}

