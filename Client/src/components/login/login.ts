import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../../services/loginService/login-service';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AuthService } from '../../services/AuthService/auth-service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, HttpClientModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  router = inject(Router);
  logInService = inject(LoginService);
  authService = inject(AuthService);
  data = {
    email: '',
    password: ''
  };

  onSubmitLogin() {
    if (!this.data.email || !this.data.password) {
      alert('נא למלא אימייל וסיסמה');
      return;
    }
    this.logInService.login(this.data.email, this.data.password).subscribe({
      next: (response: any) => {
        const user = response.User || response.user;
        if (user) {
          localStorage.setItem('Id', (user.Id || user.id));

          const nameToSave = user.FullName || user.fullName || 'משתמש';
          this.authService.updateUserName(nameToSave);
        }
        alert('התחברות הצליחה!');
        this.router.navigate(['/property-list']);
      },
      error: (err) => {
        alert('אימייל או סיסמה שגויים.');
        this.router.navigate(['/register'])
      }
    });
  }
}