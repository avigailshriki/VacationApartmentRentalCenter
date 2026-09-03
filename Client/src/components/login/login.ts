import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../../services/loginService/login-service';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AuthService } from '../../services/AuthService/auth-service';
import { ToastService } from '../../services/ToastService/toast-service';
import { GoogleSigninButtonComponent } from '../google-signin-button/google-signin-button';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, HttpClientModule, GoogleSigninButtonComponent],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  router = inject(Router);
  logInService = inject(LoginService);
  authService = inject(AuthService);
  toastService = inject(ToastService);
  data = {
    email: '',
    password: ''
  };

  onSubmitLogin() {
    if (!this.data.email || !this.data.password) {
      this.toastService.error('נא למלא אימייל וסיסמה');
      return;
    }
    this.logInService.login(this.data.email, this.data.password).subscribe({
      next: (response: any) => {
        const user = response.User || response.user;
        const token = response.Token || response.token;
        if (user && token) {
          localStorage.setItem('Id', (user.Id || user.id));

          const nameToSave = user.FullName || user.fullName || 'משתמש';
          this.authService.login(nameToSave, token);
        }
        this.toastService.success('התחברות הצליחה!');
        this.router.navigate(['/property-list']);
      },
      error: (err) => {
        this.toastService.error('אימייל או סיסמה שגויים.');
        this.router.navigate(['/register'])
      }
    });
  }

  onGoogleCredential(idToken: string) {
    this.logInService.googleLogin(idToken).subscribe({
      next: (response: any) => {
        const user = response.User || response.user;
        const token = response.Token || response.token;
        if (user && token) {
          localStorage.setItem('Id', (user.Id || user.id));

          const nameToSave = user.FullName || user.fullName || 'משתמש';
          this.authService.login(nameToSave, token);
        }
        this.toastService.success('התחברות עם גוגל הצליחה!');
        this.router.navigate(['/property-list']);
      },
      error: (err) => {
        console.error('שגיאה בהתחברות עם גוגל:', err);
        this.toastService.error('ההתחברות עם גוגל נכשלה.');
      }
    });
  }
}
