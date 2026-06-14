import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { AuthResponse, LoginRequest } from '../models/auth.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
    private readonly apiUrl = `${environment.apiUrl}/users`;

  currentUser = signal<AuthResponse | null>(this.getUserFromStorage());

  isLoggedIn = computed(() => this.currentUser() !== null);

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(this.apiUrl + '/login', credentials).pipe(
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

  getToken(): string | null {
    return this.currentUser()?.token || null;
  }

  isAuthenticated(): boolean {
    return this.isLoggedIn();
  }

  private getUserFromStorage(): AuthResponse | null {
    const session = localStorage.getItem('user_session');
    return session ? JSON.parse(session) : null;
  }
}