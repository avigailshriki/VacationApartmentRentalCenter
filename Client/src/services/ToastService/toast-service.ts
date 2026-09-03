import { Injectable, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'info';

export interface Toast {
  id: number;
  message: string;
  type: ToastType;
}

// שירות התראות מרכזי - מחליף את שימוש ה-alert() המובנה של הדפדפן בהודעות מעוצבות בתוך העמוד.
@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private nextId = 1;
  toasts = signal<Toast[]>([]);

  success(message: string, durationMs: number = 4000): void {
    this.show(message, 'success', durationMs);
  }
  error(message: string, durationMs: number = 6000): void {
    this.show(message, 'error', durationMs);
  }
  info(message: string, durationMs: number = 4000): void {
    this.show(message, 'info', durationMs);
  }
  dismiss(id: number): void {
    this.toasts.update(list => list.filter(t => t.id !== id));
  }
  private show(message: string, type: ToastType, durationMs: number): void {
    const id = this.nextId++;
    this.toasts.update(list => [...list, { id, message, type }]);
    setTimeout(() => this.dismiss(id), durationMs);
  }
}
