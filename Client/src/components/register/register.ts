import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../../services/loginService/login-service';
import { FormsModule, NgForm } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ToastService } from '../../services/ToastService/toast-service';
import { AuthService } from '../../services/AuthService/auth-service';
import { GoogleSigninButtonComponent } from '../google-signin-button/google-signin-button';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, HttpClientModule, CommonModule, GoogleSigninButtonComponent],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  router = inject(Router);
  logInService = inject(LoginService);
  toastService = inject(ToastService);
  authService = inject(AuthService);

  loginData = {
    email: '',
    password: '',
    fullName: '',
    phone: ''
  };

  toLogIn(loginForm: NgForm) {
    if (loginForm.invalid) {
      this.toastService.error('נא למלא את כל שדות החובה בצורה תקינה.');
      return;
    }
    const registerBody = {
      email: this.loginData.email,
      password: this.loginData.password,
      fullName: this.loginData.fullName,
      phone: this.loginData.phone ? this.loginData.phone.toString() : ''
    };
    this.logInService.register(registerBody).subscribe({
      next: (response: any) => {
        this.toastService.success(response.Message || response.message || 'נרשמת בהצלחה!');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('שגיאה מהשרת:', err);
        const errorMsg = err.error?.Message || err.error?.message || 'ההרשמה נכשלה.';
        this.toastService.error(errorMsg);
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
        this.toastService.success('נרשמת והתחברת בהצלחה עם גוגל!');
        this.router.navigate(['/property-list']);
      },
      error: (err) => {
        console.error('שגיאה בהרשמה עם גוגל:', err);
        this.toastService.error('ההרשמה עם גוגל נכשלה.');
      }
    });
  }
}