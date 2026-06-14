export enum StatusEnum {
  Pending = 0,
  Postponed = 1,
  Completed = 2,
  Overdue = 3
}

export interface Reminder {
  id: string;
  userId: string;
  title: string;
  description: string;
  targetDateTime: string;
  status: StatusEnum;
}

export interface ReminderRequestDto {
  userId: string;
  title: string;
  description: string;
  targetDateTime: string;
  status: StatusEnum;
}