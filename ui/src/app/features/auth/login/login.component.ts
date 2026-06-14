import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  email = '';
  password = '';

  errorMessage = signal<string | null>(null);

  onSubmit(): void {
    this.errorMessage.set(null);

    const credentials = {
      email: this.email.trim(),
      password: this.password
    };

    this.authService.login(credentials).subscribe({
      next: () => {
        this.router.navigate(['/reminders']);
      },
      error: (err) => {
        if (err.error && err.error.errors) {
          const validationErrors = Object.values(err.error.errors).flat().join(' ');
          this.errorMessage.set(validationErrors);
        } else if (err.status === 400 || err.status === 401) {
          this.errorMessage.set('Invalid username or password.');
        } else {
          this.errorMessage.set('An unexpected system error occurred. Please try again later.');
        }
      }
    });
  }
}