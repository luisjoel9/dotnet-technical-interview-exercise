import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { ReminderListComponent } from './features/reminders/reminder-list/reminder-list.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'reminders', component: ReminderListComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];