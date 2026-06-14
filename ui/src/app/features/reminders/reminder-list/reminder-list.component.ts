import { Component, inject, OnInit, signal } from '@angular/core';
import { Reminder, StatusEnum } from '../../../core/models/reminder.model';
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
  readonly reminderService = inject(ReminderService);
  readonly authService = inject(AuthService);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);

  reminders = signal<Reminder[]>([]);
  isEditing = signal(false);
  editingReminderId: string | null = null;
  
  public statusEnum = StatusEnum; 
  
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
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: [''],
      targetDateTime: ['', Validators.required]
    });
  }

  loadReminders(): void {
    this.reminderService.getReminders().subscribe({
      next: (data) => this.reminders.set(data),
      error: (err) => console.error('Error loading reminders:', err)
    });
  }

  saveReminder(): void {
    if (this.reminderForm.invalid) return;

    const formValue = this.reminderForm.value;

    if (this.isEditing() && this.editingReminderId) {
      this.reminderService.updateReminder(this.editingReminderId, formValue).subscribe({
        next: () => {
          this.loadReminders();
          this.cancelEdit();
        }
      });
    } else {
      this.reminderService.createReminder(formValue).subscribe({
        next: () => {
          this.loadReminders();
          this.reminderForm.reset();
        }
      });
    }
  }

  editReminder(item: Reminder): void {
    this.isEditing.set(true);
    this.editingReminderId = item.id;
    
    const formattedDate = new Date(item.targetDateTime).toISOString().slice(0, 16);
    
    this.reminderForm.patchValue({
      title: item.title,
      description: item.description,
      targetDateTime: formattedDate
    });
  }

  cancelEdit(): void {
    this.isEditing.set(false);
    this.editingReminderId = null;
    this.reminderForm.reset();
  }

  deleteReminder(id: string): void {
    if (confirm('Are you sure you want to delete this reminder?')) {
      this.reminderService.deleteReminder(id).subscribe({
        next: () => this.loadReminders()
      });
    }
  }

  completeReminder(item: Reminder): void {
    this.reminderService.completeReminder(item.id).subscribe({
      next: () => this.loadReminders()
    });
  }

  snoozeReminder(item: Reminder): void {
    this.reminderService.postponeReminder(item.id).subscribe({
      next: () => this.loadReminders()
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getStatusClass(status: StatusEnum): string {
    switch (status) {
      case StatusEnum.Pending: return 'border-pending';
      case StatusEnum.Postponed: return 'border-postponed';
      case StatusEnum.Completed: return 'border-completed';
      case StatusEnum.Overdue: return 'border-overdue';
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