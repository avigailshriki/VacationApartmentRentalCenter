import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/AuthService/auth-service';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './nav.html',
  styleUrl: './nav.css'
})
export class Nav {
  router = inject(Router);
  authService = inject(AuthService);
  currentName: string = '';
  userName = this.authService.userName$;

  logout() {
    localStorage.clear();
    this.authService.logout();
    this.authService.updateUserName('');
    this.router.navigate(['/property-list']);
  }
  ngOnInit() {
    this.authService.userName$.subscribe(name => {
      this.currentName = name;
    });
  }
  getUserName(): string | null {
    return localStorage.getItem('userName');
  }
  getOwnerID(): string | null {
    return localStorage.getItem('Id');
  }
}