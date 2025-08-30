import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { App } from './app';
import { AuthService } from './services/auth.service';

describe('App', () => {
  const authMock = {
    isAuthenticated: () => true,
    getCurrentUser: () => ({ name: 'Test User' }),
    logout: () => { }
  } as Partial<AuthService> as AuthService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        HttpClientTestingModule
      ],
      declarations: [App],
      providers: [
        { provide: AuthService, useValue: authMock }
      ]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render navbar brand when authenticated', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const brand = compiled.querySelector('.navbar-brand');
    expect(brand?.textContent).toContain('Legal System');
  });
});
