import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { Router } from '@angular/router';
import { ApiService } from './api.service';
import { LoginRequest, RegistrationRequest, AuthResponse, User, JwtPayload } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private apiService: ApiService,
    private router: Router
  ) {
    this.initializeAuth();
  }

  private initializeAuth(): void {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payload = this.decodeToken(token);
        if (payload && payload.exp > Date.now() / 1000) {
          const user: User = {
            id: payload.UserId,
            name: payload.Name,
            userName: payload.UserName
          };
          this.currentUserSubject.next(user);
          this.isAuthenticatedSubject.next(true);
        } else {
          this.clearAuth();
        }
      } catch (error) {
        this.clearAuth();
      }
    }
  }

  private decodeToken(token: string): JwtPayload | null {
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload));
    } catch (error) {
      return null;
    }
  }

  login(loginRequest: LoginRequest): Observable<AuthResponse> {
    return this.apiService.executeCommand<AuthResponse>('ADMIN', 'LogInCommand', loginRequest)
      .pipe(
        tap(response => {
          if (response.success && response.result) {
            localStorage.setItem('token', response.result.token);
            this.currentUserSubject.next(response.result);
            this.isAuthenticatedSubject.next(true);
          }
        })
      );
  }

  register(registrationRequest: RegistrationRequest): Observable<AuthResponse> {
    return this.apiService.executeCommand<AuthResponse>('ADMIN', 'RegistrationCommand', registrationRequest)
      .pipe(
        tap(response => {
          if (response.success && response.result) {
            localStorage.setItem('token', response.result.token);
            this.currentUserSubject.next(response.result);
            this.isAuthenticatedSubject.next(true);
          }
        })
      );
  }

  logout(): void {
    this.clearAuth();
    this.router.navigate(['/login']);
  }

  clearAuth(): void {
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }
}
