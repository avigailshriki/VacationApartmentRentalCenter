import { Component, inject, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PropertyService } from '../../services/PropertiesService/property-service';
import { ToastService } from '../../services/ToastService/toast-service';
import { ConfirmService } from '../../services/ConfirmService/confirm-service';
import { IPropertyAvailability } from '../../Interfaces/Iproperty';

interface CalendarDay {
  date: Date;
  inCurrentMonth: boolean;
  isPast: boolean;
  isBlocked: boolean;
}

// רכיב לוח שנה להצגת/ניהול זמינות של נכס.
// editable=false (עמוד פרטי נכס): תצוגה בלבד - איזה תאריכים תפוסים.
// editable=true (עריכת נכס): הבעלים יכול לחסום ולבטל חסימת טווחי תאריכים.
@Component({
  selector: 'app-availability-calendar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './availability-calendar.html',
  styleUrl: './availability-calendar.css'
})
export class AvailabilityCalendar implements OnInit, OnChanges {
  @Input() propertyId!: number;
  @Input() editable: boolean = false;

  propertyService = inject(PropertyService);
  toastService = inject(ToastService);
  confirmService = inject(ConfirmService);

  ranges: IPropertyAvailability[] = [];
  calendarDays: CalendarDay[] = [];
  currentMonth = new Date().getMonth();
  currentYear = new Date().getFullYear();
  loading = false;

  monthNames = ['ינואר', 'פברואר', 'מרץ', 'אפריל', 'מאי', 'יוני', 'יולי', 'אוגוסט', 'ספטמבר', 'אוקטובר', 'נובמבר', 'דצמבר'];
  weekDays = ['א׳', 'ב׳', 'ג׳', 'ד׳', 'ה׳', 'ו׳', 'ש׳'];

  ngOnInit() {
    this.loadAvailability();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['propertyId'] && !changes['propertyId'].firstChange) {
      this.loadAvailability();
    }
  }

  loadAvailability() {
    if (!this.propertyId) return;
    this.loading = true;
    this.propertyService.getAvailability(this.propertyId).subscribe({
      next: (data) => {
        this.ranges = data || [];
        this.buildCalendar();
        this.loading = false;
      },
      error: (err) => {
        console.error('שגיאה בטעינת לוח הזמינות:', err);
        this.loading = false;
      }
    });
  }

  private buildCalendar() {
    const firstOfMonth = new Date(this.currentYear, this.currentMonth, 1);
    const startDayOfWeek = firstOfMonth.getDay();
    const daysInMonth = new Date(this.currentYear, this.currentMonth + 1, 0).getDate();
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const days: CalendarDay[] = [];

    for (let i = 0; i < startDayOfWeek; i++) {
      const d = new Date(this.currentYear, this.currentMonth, 1 - (startDayOfWeek - i));
      days.push(this.makeDay(d, false, today));
    }
    for (let d = 1; d <= daysInMonth; d++) {
      days.push(this.makeDay(new Date(this.currentYear, this.currentMonth, d), true, today));
    }
    while (days.length % 7 !== 0) {
      const last = days[days.length - 1].date;
      const d = new Date(last);
      d.setDate(d.getDate() + 1);
      days.push(this.makeDay(d, false, today));
    }
    this.calendarDays = days;
  }

  private makeDay(date: Date, inCurrentMonth: boolean, today: Date): CalendarDay {
    const isBlocked = this.ranges.some(r => {
      const start = new Date(r.StartDate);
      start.setHours(0, 0, 0, 0);
      const end = new Date(r.EndDate);
      end.setHours(0, 0, 0, 0);
      const d = new Date(date);
      d.setHours(0, 0, 0, 0);
      return d >= start && d <= end;
    });
    return {
      date,
      inCurrentMonth,
      isPast: date < today,
      isBlocked
    };
  }

  previousMonth() {
    this.currentMonth--;
    if (this.currentMonth < 0) {
      this.currentMonth = 11;
      this.currentYear--;
    }
    this.buildCalendar();
  }

  nextMonth() {
    this.currentMonth++;
    if (this.currentMonth > 11) {
      this.currentMonth = 0;
      this.currentYear++;
    }
    this.buildCalendar();
  }

  blockRange(startInput: HTMLInputElement, endInput: HTMLInputElement) {
    const start = startInput.value;
    const end = endInput.value;
    if (!start || !end) {
      this.toastService.error('יש לבחור תאריך התחלה ותאריך סיום.');
      return;
    }
    if (new Date(end) < new Date(start)) {
      this.toastService.error('תאריך הסיום חייב להיות אחרי תאריך ההתחלה.');
      return;
    }
    this.propertyService.blockDates(this.propertyId, start, end).subscribe({
      next: () => {
        this.toastService.success('התאריכים נחסמו בהצלחה.');
        startInput.value = '';
        endInput.value = '';
        this.loadAvailability();
      },
      error: (err) => {
        console.error('שגיאה בחסימת תאריכים:', err);
        const message = typeof err?.error === 'string' ? err.error : 'שגיאה בחסימת התאריכים.';
        this.toastService.error(message);
      }
    });
  }

  async unblockRange(rangeId: number) {
    const confirmed = await this.confirmService.confirm('לבטל את החסימה עבור טווח התאריכים הזה?', 'ביטול חסימה', 'סגור');
    if (!confirmed) return;

    this.propertyService.unblockDates(rangeId).subscribe({
      next: () => {
        this.toastService.success('החסימה בוטלה בהצלחה.');
        this.loadAvailability();
      },
      error: (err) => {
        console.error('שגיאה בביטול חסימה:', err);
        this.toastService.error('שגיאה בביטול החסימה.');
      }
    });
  }
}
