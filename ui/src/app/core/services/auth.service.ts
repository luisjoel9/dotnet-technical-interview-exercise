import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { AuthResponse, LoginRequest } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5000/api/users/login';

  currentUser = signal<AuthResponse | null>(this.getUserFromStorage());

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(this.apiUrl, credentials).pipe(
      tap((response) => {
        localStorage.setItem('user_session', JSON.stringify(response));
        this.currentUser.set(response);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('user_session');
    this.currentUser.set(null);
  }

  isAuthenticated(): boolean {
    return this.currentUser() !== null;
  }

  private getUserFromStorage(): AuthResponse | null {
    const session = localStorage.getItem('user_session');
    return session ? JSON.parse(session) : null;
  }
}