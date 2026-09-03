import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/userService/user-service';
import { IOwner } from '../../Interfaces/Iproperty';
import { AuthService } from '../../services/AuthService/auth-service';
import { ToastService } from '../../services/ToastService/toast-service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './user.html',
  styleUrl: './user.css'
})
export class User implements OnInit {
  userService = inject(UserService);
  authService = inject(AuthService)
  toastService = inject(ToastService);
  cdr = inject(ChangeDetectorRef);
  id: number = 0;
  firstName: string = '';
  lastName: string = '';
  user: IOwner = {
    Id: 0, PhoneNumber: '',
    Email: ''
  };
  ngOnInit(): void {
    const ownerId = localStorage.getItem('Id');
    if (ownerId) {
      this.id = Number(ownerId);
      this.userService.getOwnerById(this.id).subscribe({
        next: (data: IOwner) => {
          this.user = data; // השרת מחזיר FullName
          const names = data.FullName ? data.FullName.split(' ') : [];
          this.user.FirstName = names[0] || '';
          this.user.LastName = names.slice(1).join(' ') || '';
          this.cdr.detectChanges();
        }
      });
    }
  }
  updateUser(): void {
    this.user.FullName = `${this.user.FirstName} ${this.user.LastName}`.trim();
    this.userService.update(this.id, this.user).subscribe({
      next: (res) => {
        this.authService.updateUserName(this.user.FirstName || '');
        this.toastService.success('הפרטים עודכנו בהצלחה!');
      },
      error: (err) => {
        console.error(err);
        this.toastService.error('אירעה שגיאה בעדכון הפרטים.');
      }
    });
  }
}