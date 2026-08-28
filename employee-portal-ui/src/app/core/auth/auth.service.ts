import { Injectable } from '@angular/core';
import { Apollo } from 'apollo-angular';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { LOGIN_MUTATION } from './auth.mutations';
import { AuthToken, LoginRequest, LoginResponse } from './auth.models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly tokenKey = 'employee_portal_token';
  private readonly expiryKey = 'employee_portal_token_expiry';

  constructor(private apollo: Apollo) {}

  login(credentials: LoginRequest): Observable<AuthToken> {
    return this.apollo
      .mutate<LoginResponse>({
        mutation: LOGIN_MUTATION,
        variables: {
          input: credentials,
        },
      })
      .pipe(
        map((result) => {
          const authToken = result.data?.login;

          if (!authToken) {
            throw new Error('Login failed. Authentication token was not returned.');
          }

          this.storeToken(authToken);

          return authToken;
        }),

        catchError((error) => {
          console.error('Login failed:', error);

          return throwError(() => new Error(this.getErrorMessage(error)));
        }),
      );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.expiryKey);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();

    if (!token) {
      return false;
    }

    const expiry = localStorage.getItem(this.expiryKey);

    if (!expiry) {
      return false;
    }

    const expiryTime = new Date(expiry).getTime();

    if (Number.isNaN(expiryTime)) {
      this.logout();
      return false;
    }

    if (Date.now() >= expiryTime) {
      this.logout();
      return false;
    }

    return true;
  }

  private storeToken(authToken: AuthToken): void {
    localStorage.setItem(this.tokenKey, authToken.accessToken);

    localStorage.setItem(this.expiryKey, authToken.expiresAtUtc);
  }

  private getErrorMessage(error: any): string {
    if (error?.graphQLErrors?.length) {
      return error.graphQLErrors.map((e: any) => e.message).join(', ');
    }

    if (error?.networkError) {
      return 'Unable to connect to the authentication server.';
    }

    if (error?.message) {
      return error.message;
    }

    return 'Login failed. Please check your username and password.';
  }
}
