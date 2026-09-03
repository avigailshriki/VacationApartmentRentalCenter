import { Injectable, signal } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

const isBrowser = typeof window !== 'undefined' && typeof localStorage !== 'undefined';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private userNameSubject = new BehaviorSubject<string>(isBrowser ? (localStorage.getItem('userName') || '') : '');
  userName = signal<string | null>(isBrowser ? localStorage.getItem('userName') : null);
  userName$ = this.userNameSubject.asObservable();

  updateUserName(newName: string) {
    if (isBrowser) {
      localStorage.setItem('userName', newName);
    }
    this.userNameSubject.next(newName);
    this.userName.set(newName);
  }

  // נקראת אחרי התחברות מוצלחת - שומרת גם את שם המשתמש וגם את טוקן ה-JWT שהשרת החזיר.
  login(name: string, token: string) {
    if (isBrowser) {
      localStorage.setItem('userName', name);
      localStorage.setItem('token', token);
    }
    this.userNameSubject.next(name);
    this.userName.set(name);
  }

  logout() {
    if (isBrowser) {
      localStorage.clear();
    }
    this.userNameSubject.next('');
    this.userName.set(null);
  }

  getToken(): string | null {
    return isBrowser ? localStorage.getItem('token') : null;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
