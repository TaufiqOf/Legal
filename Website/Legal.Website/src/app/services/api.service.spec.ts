import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ApiService } from './api.service';

describe('ApiService', () => {
  let service: ApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ApiService]
    });

    service = TestBed.inject(ApiService);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should execute command with auth header when token present', () => {
    localStorage.setItem('token', 'abc');
    service.executeCommand<any>('ADMIN', 'DoThing', { a: 1 }).subscribe();

    const req = httpMock.expectOne(r => r.url.includes('/Command/Execute/ADMIN'));
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('Authorization')).toBe('Bearer abc');
    req.flush({});
  });

  it('should execute query without auth header when no token', () => {
    service.executeQuery<any>('ADMIN', 'GetThing', { id: 1 }).subscribe();
    const req = httpMock.expectOne(r => r.url.includes('/Query/Execute/ADMIN'));
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush({});
  });
});
