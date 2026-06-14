import { inject, Injectable } from '@angular/core';
import { Reminder, ReminderRequestDto } from '../models/reminder.model';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment.prod';

@Injectable({
  providedIn: 'root'
})
export class ReminderService {
  private http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/reminders`;

  getReminders(isCompleted?: boolean, isOverdue?: boolean): Observable<Reminder[]> {
    let params = new HttpParams();
    if (isCompleted !== undefined) params = params.set('isCompleted', isCompleted.toString());
    if (isOverdue !== undefined) params = params.set('isOverdue', isOverdue.toString());

    return this.http.get<Reminder[]>(this.apiUrl, { params });
  }

  getReminderById(id: string): Observable<Reminder> {
    return this.http.get<Reminder>(`${this.apiUrl}/${id}`);
  }

  createReminder(reminder: ReminderRequestDto): Observable<Reminder> {
    return this.http.post<Reminder>(this.apiUrl, reminder);
  }

  updateReminder(id: string, reminder: ReminderRequestDto): Observable<boolean> {
    return this.http.put<boolean>(`${this.apiUrl}/${id}`, reminder);
  }

  deleteReminder(id: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`);
  }

  completeReminder(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/complete`, {});
  }

  postponeReminder(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/postpone`, {});
  }
}