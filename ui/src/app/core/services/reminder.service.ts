import { inject, Injectable } from '@angular/core';
import { Reminder, ReminderRequestDto } from '../models/reminder.model';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ReminderService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5000/api/reminders';

  getAll(isCompleted?: boolean, isOverdue?: boolean): Observable<Reminder[]> {
    const params: any = {};
    if (isCompleted !== undefined) params.isCompleted = isCompleted;
    if (isOverdue !== undefined) params.isOverdue = isOverdue;
    return this.http.get<Reminder[]>(this.apiUrl, { params });
  }

  getById(id: string): Observable<Reminder> {
    return this.http.get<Reminder>(`${this.apiUrl}/${id}`);
  }

  create(reminder: ReminderRequestDto): Observable<Reminder> {
    return this.http.post<Reminder>(this.apiUrl, reminder);
  }

  update(id: string, reminder: ReminderRequestDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, reminder);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}