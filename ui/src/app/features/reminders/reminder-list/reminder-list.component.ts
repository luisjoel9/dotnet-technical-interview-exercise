import { Component, inject, OnInit, signal } from '@angular/core';
import { Reminder, ReminderRequestDto, StatusEnum } from '../../../core/models/reminder.model';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ReminderService } from '../../../core/services/reminder.service';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-reminder-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DatePipe],
  templateUrl: './reminder-list.component.html',
  styleUrl: './reminder-list.component.scss'
})
export class ReminderListComponent implements OnInit {
  reminderService = inject(ReminderService);
  authService = inject(AuthService);
  private fb = inject(FormBuilder);
  private router = inject(Router);

  reminders = signal<Reminder[]>([]);
  isEditing = signal<boolean>(false);
  editingReminderId: string | null = null;

  editingReminderStatus: StatusEnum = StatusEnum.Pending; 
  statusEnum = StatusEnum;

  reminderForm!: FormGroup;

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    this.initForm();
    this.loadReminders();
  }

  initForm(): void {
    this.reminderForm = this.fb.group({
      title: ['', [Validators.required]],
      description: [''],
      targetDateTime: ['', [Validators.required]]
    });
  }

  loadReminders(): void {
    this.reminderService.getAll().subscribe((data: any[]) => {
      const currentUserId = this.authService.currentUser()?.id;
      this.reminders.set(data.filter(r => r.userId === currentUserId));
    });
  }

  saveReminder(): void {
    const currentUserId = this.authService.currentUser()?.id;
    if (!currentUserId) return;

    const payload: ReminderRequestDto = {
      userId: currentUserId,
      title: this.reminderForm.value.title,
      description: this.reminderForm.value.description,
      targetDateTime: new Date(this.reminderForm.value.targetDateTime).toISOString(),
      status: this.isEditing() ? this.editingReminderStatus : StatusEnum.Pending 
    };

    if (this.isEditing() && this.editingReminderId) {
      this.reminderService.update(this.editingReminderId, payload).subscribe(() => {
        this.cancelEdit();
        this.loadReminders();
      });
    } else {
      this.reminderService.create(payload).subscribe(() => {
        this.reminderForm.reset();
        this.loadReminders();
      });
    }
  }

  editReminder(reminder: Reminder): void {
    this.isEditing.set(true);
    this.editingReminderId = reminder.id;
    this.editingReminderStatus = reminder.status;
    
    const formattedDate = new Date(reminder.targetDateTime).toISOString().slice(0, 16);
    
    this.reminderForm.patchValue({
      title: reminder.title,
      description: reminder.description,
      targetDateTime: formattedDate
    });
  }

  cancelEdit(): void {
    this.isEditing.set(false);
    this.editingReminderId = null;
    this.editingReminderStatus = StatusEnum.Pending;
    this.reminderForm.reset();
  }

  deleteReminder(id: string): void {
    if (confirm('Are you sure you want to delete this reminder?')) {
      this.reminderService.delete(id).subscribe(() => this.loadReminders());
    }
  }

  completeReminder(reminder: Reminder): void {
    const payload: ReminderRequestDto = { 
      userId: reminder.userId,
      title: reminder.title,
      description: reminder.description,
      targetDateTime: new Date(reminder.targetDateTime).toISOString(),
      status: StatusEnum.Completed 
    };
    this.reminderService.update(reminder.id, payload).subscribe(() => this.loadReminders());
  }

  snoozeReminder(reminder: Reminder): void {
    const originalDate = new Date(reminder.targetDateTime);
    const postponedDate = new Date(originalDate.getTime() + 15 * 60000);

    const payload: ReminderRequestDto = {
      userId: reminder.userId,
      title: reminder.title,
      description: reminder.description,
      targetDateTime: postponedDate.toISOString(),
      status: StatusEnum.Postponed
    };

    this.reminderService.update(reminder.id, payload).subscribe(() => this.loadReminders());
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getStatusClass(status: StatusEnum): string {
    switch (status) {
      case StatusEnum.Pending: return 'status-pending';
      case StatusEnum.Postponed: return 'status-postponed';
      case StatusEnum.Completed: return 'status-completed';
      case StatusEnum.Overdue: return 'status-overdue';
      default: return '';
    }
  }

  getStatusLabel(status: StatusEnum): string {
    switch (status) {
      case StatusEnum.Pending: return 'Pending';
      case StatusEnum.Postponed: return 'Postponed';
      case StatusEnum.Completed: return 'Completed';
      case StatusEnum.Overdue: return 'Overdue';
      default: return 'Unknown';
    }
  }
}