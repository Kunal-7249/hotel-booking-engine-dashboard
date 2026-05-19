import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideRouter, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { AuthResponse } from '../models/auth.model';
import { environment } from '../../../environments/environment';

// ── localStorage mock — defined at module level ───────
const localStorageMock = window.localStorage;

// ── Sample Data — module level ────────────────────────
const mockAuthResponse: AuthResponse = {
  token: 'mock.jwt.token',
  username: 'kunal',
  email: 'kunal@gmail.com',
  role: 'ExternalUser',
  expiresAt: '2026-05-15T00:00:00'
};

const adminAuthResponse: AuthResponse = {
  token: 'mock.jwt.token',
  username: 'admin',
  email: 'admin@gmail.com',
  role: 'Admin',
  expiresAt: '2026-05-15T00:00:00'
};

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.apiUrl}/api/auth`;

  beforeEach(() => {
    localStorageMock.clear();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(withFetch()),
        provideHttpClientTesting(),
        provideRouter([])
      ]
    });
    const router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate').mockResolvedValue(true);
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorageMock.clear();
  });

  // ── login Tests ───────────────────────────────────────

  it('should login and store token in localStorage', () => {
    service.login({ email: 'kunal@gmail.com', password: 'pass123' })
      .subscribe(() => {
        expect(localStorageMock.getItem('auth_token')).toBe('mock.jwt.token');
        expect(service.isLoggedIn()).toBe(true);
      });

    const req = httpMock.expectOne(`${apiUrl}/login`);
    expect(req.request.method).toBe('POST');
    req.flush(mockAuthResponse);
  });

  it('should set currentUser signal on login', () => {
    service.login({ email: 'kunal@gmail.com', password: 'pass123' })
      .subscribe(() => {
        expect(service.currentUser()?.username).toBe('kunal');
        expect(service.currentUser()?.role).toBe('ExternalUser');
      });

    const req = httpMock.expectOne(`${apiUrl}/login`);
    req.flush(mockAuthResponse);
  });

  it('should send POST request with correct payload on login', () => {
    service.login({ email: 'kunal@gmail.com', password: 'pass123' }).subscribe();

    const req = httpMock.expectOne(`${apiUrl}/login`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({
      email: 'kunal@gmail.com',
      password: 'pass123'
    });
    req.flush(mockAuthResponse);
  });

  // ── register Tests ────────────────────────────────────

  it('should register and store token in localStorage', () => {
    service.register({
      username: 'kunal',
      email: 'kunal@gmail.com',
      password: 'pass123',
      role: 'ExternalUser'
    }).subscribe(() => {
      expect(localStorageMock.getItem('auth_token')).toBe('mock.jwt.token');
      expect(service.isLoggedIn()).toBe(true);
    });

    const req = httpMock.expectOne(`${apiUrl}/register`);
    expect(req.request.method).toBe('POST');
    req.flush(mockAuthResponse);
  });

  it('should set currentUser signal on register', () => {
    service.register({
      username: 'kunal',
      email: 'kunal@gmail.com',
      password: 'pass123',
      role: 'ExternalUser'
    }).subscribe(() => {
      expect(service.currentUser()?.username).toBe('kunal');
    });

    const req = httpMock.expectOne(`${apiUrl}/register`);
    req.flush(mockAuthResponse);
  });

  // ── logout Tests ──────────────────────────────────────

  it('should clear localStorage and signals on logout', () => {
  const router = TestBed.inject(Router);
  vi.spyOn(router, 'navigate').mockResolvedValue(true);  // ← mock navigate

  service.login({ email: 'kunal@gmail.com', password: 'pass123' })
    .subscribe(() => {
      service.logout();
      expect(window.localStorage.getItem('auth_token')).toBeNull();
      expect(service.isLoggedIn()).toBe(false);
      expect(service.currentUser()).toBeNull();
    });

  const req = httpMock.expectOne(`${apiUrl}/login`);
  req.flush(mockAuthResponse);
  });

  // ── isAdmin Tests ─────────────────────────────────────

  it('should return true for isAdmin when role is Admin', () => {
    service.login({ email: 'admin@gmail.com', password: 'pass123' })
      .subscribe(() => {
        expect(service.isAdmin()).toBe(true);
      });

    const req = httpMock.expectOne(`${apiUrl}/login`);
    req.flush(adminAuthResponse);
  });

  it('should return false for isAdmin when role is ExternalUser', () => {
    service.login({ email: 'kunal@gmail.com', password: 'pass123' })
      .subscribe(() => {
        expect(service.isAdmin()).toBe(false);
      });

    const req = httpMock.expectOne(`${apiUrl}/login`);
    req.flush(mockAuthResponse);
  });

  // ── getToken Tests ────────────────────────────────────

  it('should return token from localStorage after login', () => {
    service.login({ email: 'kunal@gmail.com', password: 'pass123' })
      .subscribe(() => {
        expect(service.getToken()).toBe('mock.jwt.token');
      });

    const req = httpMock.expectOne(`${apiUrl}/login`);
    req.flush(mockAuthResponse);
  });

  it('should return null when not logged in', () => {
    expect(service.getToken()).toBeNull();
    expect(service.isLoggedIn()).toBe(false);
  });

  it('should return null for getRole when not logged in', () => {
    expect(service.getRole()).toBeNull();
  });

  // ── session restore Tests ─────────────────────────────

  it('should restore session from localStorage on init', () => {
  window.localStorage.setItem('auth_token', 'mock.jwt.token');
  window.localStorage.setItem('auth_user', JSON.stringify(mockAuthResponse));

  TestBed.resetTestingModule();
  TestBed.configureTestingModule({
    providers: [
      AuthService,
      provideHttpClient(withFetch()),
      provideHttpClientTesting(),
      provideRouter([])
    ]
  });

  const newService = TestBed.inject(AuthService);

  expect(newService.isLoggedIn()).toBe(true);
  expect(newService.currentUser()?.username).toBe('kunal');
  });

  it('should not restore session when localStorage is empty', () => {
    expect(service.isLoggedIn()).toBe(false);
    expect(service.currentUser()).toBeNull();
  });
});