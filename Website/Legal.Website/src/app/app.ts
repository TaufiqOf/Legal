import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css',
  standalone: false // ensure this component can be declared in NgModule
})
export class App {
  isNavCollapsed = true;
  userMenuOpen = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  getCurrentUser() {
    return this.authService.getCurrentUser();
  }

  logout(): void {
    this.authService.logout();
    this.userMenuOpen = false;
  }

  isLoginPage(): boolean {
    return this.router.url === '/login' || this.router.url === '/register';
  }
}
