import { AfterViewInit, Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { environment } from '../../environments/environment';

declare var google: any;

type GoogleButtonText = 'signin_with' | 'signup_with' | 'continue_with';

// כפתור "התחברות עם גוגל" משותף - מאתחל את Google Identity Services ומרנדר את הכפתור הרשמי של גוגל,
// עם עיצוב (צבע/צורה/רוחב) שמותאם לשאר האתר ככל שגוגל מאפשרת להתאים אישית.
// כשההתחברות מצליחה בדפדפן, גוגל מחזירה credential (ID Token) שמועבר החוצה דרך ה-Output,
// והרכיב שמשתמש בכפתור (login/register) אחראי לשלוח אותו לשרת ולהתחבר בפועל.
@Component({
  selector: 'app-google-signin-button',
  standalone: true,
  imports: [],
  templateUrl: './google-signin-button.html',
  styleUrl: './google-signin-button.css'
})
export class GoogleSigninButtonComponent implements AfterViewInit {
  @ViewChild('googleButton') googleButton!: ElementRef;
  @Output() credential = new EventEmitter<string>();

  // "signin_with" בדף התחברות, "signup_with" בדף הרשמה - כדי שהטקסט על הכפתור יתאים להקשר.
  @Input() text: GoogleButtonText = 'continue_with';
  // רוחב הכפתור בפיקסלים (מוגבל על ידי גוגל לטווח 200-400).
  @Input() width: number = 320;

  ngAfterViewInit(): void {
    this.tryInit();
  }

  private tryInit(attemptsLeft: number = 20): void {
    const isBrowser = typeof window !== 'undefined';
    if (!isBrowser) return;

    if (!environment.googleClientId) {
      console.warn('googleClientId לא מוגדר ב-environment - כפתור ההתחברות עם גוגל לא יוצג.');
      return;
    }

    if (typeof google === 'undefined' || !google.accounts?.id) {
      // הסקריפט של גוגל נטען אסינכרונית (async/defer) - ייתכן שהוא עדיין לא נטען, מנסים שוב בעוד רגע.
      if (attemptsLeft > 0) {
        setTimeout(() => this.tryInit(attemptsLeft - 1), 200);
      }
      return;
    }

    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: (response: any) => this.credential.emit(response.credential)
    });

    google.accounts.id.renderButton(this.googleButton.nativeElement, {
      type: 'standard',
      theme: 'outline',
      size: 'large',
      shape: 'pill',
      text: this.text,
      logo_alignment: 'center',
      width: this.width,
      locale: 'he'
    });
  }
}
