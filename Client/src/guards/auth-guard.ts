import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/AuthService/auth-service';

// חוסם ניווט למסכים שדורשים התחברות (הוספת נכס, הדירות שלי, איזור אישי)
// ומעביר למסך ההתחברות אם אין טוקן שמור.
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};
