import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  loading = false;
  error: string | null = null;

  readonly form = this.fb.nonNullable.group({
    userNameOrEmail: ['', [Validators.required]],
    password: ['', [Validators.required]],
  });

  async onSubmit(): Promise<void> {
    this.error = null;
    if (this.form.invalid) return;

    this.loading = true;
    try {
      const { userNameOrEmail, password } = this.form.getRawValue();
      await this.auth.login({ userNameOrEmail, password });
      await this.router.navigateByUrl('/home');
    } catch {
      this.error = 'Invalid username/email or password.';
    } finally {
      this.loading = false;
    }
  }
}

