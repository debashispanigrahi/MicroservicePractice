import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  loading = false;

  showPassword = false;

  errorMessage = '';

  loginForm = this.fb.nonNullable.group({
    userName: ['', Validators.required],
    password: ['', Validators.required],
  });

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  login(): void {
    this.errorMessage = '';

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();

      return;
    }

    this.loading = true;

    const { userName, password } = this.loginForm.getRawValue();

    this.authService.login({ userName, password }).subscribe({
      next: () => {
        this.loading = false;

        // Authentication succeeded.
        // AuthService has already stored the token.

        this.router.navigate(['/employees']);
      },

      error: (error) => {
        console.error('Login failed:', error);

        this.loading = false;

        this.errorMessage = this.getErrorMessage(error);
      },
    });
  }

  private getErrorMessage(error: unknown): string {
    if (error && typeof error === 'object' && 'message' in error) {
      const message = (error as { message?: string }).message;

      if (message) {
        return message;
      }
    }

    return 'Invalid username or password. Please try again.';
  }
}
