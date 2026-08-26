import { Injectable, signal } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private userNameSubject = new BehaviorSubject<string>(localStorage.getItem('userName') || '');
  userName = signal<string | null>(localStorage.getItem('userName'));
  userName$ = this.userNameSubject.asObservable();

  updateUserName(newName: string) {
    localStorage.setItem('userName', newName);
    this.userNameSubject.next(newName);
  }
  login(name: string) {
    localStorage.setItem('userName', name);
    this.userName.set(name); 
  }
  logout() {
    localStorage.clear();
    this.userName.set(null); 
  }
}