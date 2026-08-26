import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../../services/loginService/login-service';
import { FormsModule, NgForm } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, HttpClientModule, CommonModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  router = inject(Router);
  logInService = inject(LoginService);

  loginData = {
    email: '',
    password: '',
    fullName: '',
    phone: ''
  };

  toLogIn(loginForm: NgForm) {
    if (loginForm.invalid) {
      alert('נא למלא את כל שדות החובה בצורה תקינה.');
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
        alert(response.Message || response.message || 'נרשמת בהצלחה!');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('שגיאה מהשרת:', err);
        const errorMsg = err.error?.Message || err.error?.message || 'ההרשמה נכשלה.';
        alert(errorMsg);
      }
    });
  }
}