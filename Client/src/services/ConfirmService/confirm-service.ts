import { Injectable, signal } from '@angular/core';

export interface ConfirmRequest {
  message: string;
  confirmText: string;
  cancelText: string;
  resolve: (result: boolean) => void;
}

// שירות אישור מרכזי - מחליף את שימוש ה-confirm() המובנה של הדפדפן בחלונית מעוצבת בתוך העמוד.
// שימוש: const ok = await this.confirmService.confirm('האם למחוק?');
@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  request = signal<ConfirmRequest | null>(null);

  confirm(message: string, confirmText: string = 'אישור', cancelText: string = 'ביטול'): Promise<boolean> {
    return new Promise<boolean>((resolve) => {
      this.request.set({ message, confirmText, cancelText, resolve });
    });
  }

  respond(result: boolean): void {
    const current = this.request();
    if (current) {
      current.resolve(result);
      this.request.set(null);
    }
  }
}
