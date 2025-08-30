import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { AuthService } from './auth.service';
import { ApiService } from './api.service';

describe('AuthService', () => {
  let service: AuthService; // assigned in each test when needed
  let apiSpy: jasmine.SpyObj<ApiService>;
  let routerSpy: jasmine.SpyObj<Router>;

  const buildToken = (payloadObj: any) => {
    const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
    const payload = btoa(JSON.stringify(payloadObj));
    return `${header}.${payload}.signature`;
  };

  const createService = () => TestBed.inject(AuthService);

  beforeEach(() => {
    apiSpy = jasmine.createSpyObj<ApiService>('ApiService', ['executeCommand']);
    routerSpy = jasmine.createSpyObj<Router>('Router', ['navigate']);
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        { provide: ApiService, useValue: apiSpy },
        { provide: Router, useValue: routerSpy },
        AuthService
      ]
    });
  });

  it('should start unauthenticated with no token', () => {
    service = createService();
    expect(service.isAuthenticated()).toBeFalse();
    expect(service.getCurrentUser()).toBeNull();
  });

  it('should initialize with valid token', () => {
    const future = Math.floor(Date.now() / 1000) + 3600;
    const token = buildToken({ exp: future, UserId: '1', Name: 'Tester', UserName: 'tester' });
    localStorage.setItem('token', token);

    service = createService();

    expect(service.isAuthenticated()).toBeTrue();
    expect(service.getCurrentUser()?.name).toBe('Tester');
  });

  it('should clear expired token on initialize', () => {
    const past = Math.floor(Date.now() / 1000) - 10;
    const token = buildToken({ exp: past, UserId: '1', Name: 'Old', UserName: 'old' });
    localStorage.setItem('token', token);

    service = createService();

    expect(service.isAuthenticated()).toBeFalse();
    expect(localStorage.getItem('token')).toBeNull();
  });

  it('login should persist token and user on success', () => {
    service = createService();
    const response = { success: true, result: { token: 'tok123', id: '1', name: 'User', userName: 'user' } } as any;
    apiSpy.executeCommand.and.returnValue(of(response));

    service.login({ userName: 'user', password: 'pw', moduleName: 0 } as any).subscribe();

    expect(apiSpy.executeCommand).toHaveBeenCalled();
    expect(localStorage.getItem('token')).toBe('tok123');
    expect(service.isAuthenticated()).toBeTrue();
    expect(service.getCurrentUser()?.name).toBe('User');
  });

  it('register should persist token and user on success', () => {
    service = createService();
    const response = { success: true, result: { token: 'regtok', id: '2', name: 'Reg', userName: 'reg' } } as any;
    apiSpy.executeCommand.and.returnValue(of(response));

    service.register({ userName: 'reg', password: 'pw', name: 'Reg', moduleName: 0 } as any).subscribe();

    expect(apiSpy.executeCommand).toHaveBeenCalled();
    expect(localStorage.getItem('token')).toBe('regtok');
    expect(service.isAuthenticated()).toBeTrue();
    expect(service.getCurrentUser()?.name).toBe('Reg');
  });

  it('logout should clear auth state and navigate to login', () => {
    service = createService();
    const response = { success: true, result: { token: 'tok', id: '1', name: 'User', userName: 'user' } } as any;
    apiSpy.executeCommand.and.returnValue(of(response));
    service.login({ userName: 'user', password: 'pw', moduleName: 0 } as any).subscribe();

    service.logout();

    expect(service.isAuthenticated()).toBeFalse();
    expect(service.getCurrentUser()).toBeNull();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('clearAuth should remove token and reset subjects', () => {
    service = createService();
    localStorage.setItem('token', 'tok');
    (service as any).currentUserSubject.next({ id: '1', name: 'X', userName: 'x' });
    (service as any).isAuthenticatedSubject.next(true);

    service.clearAuth();

    expect(localStorage.getItem('token')).toBeNull();
    expect(service.isAuthenticated()).toBeFalse();
    expect(service.getCurrentUser()).toBeNull();
  });
});
