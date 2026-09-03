import { Component } from '@angular/core';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router'; 
import { Nav } from '../components/nav/nav/nav';
import { HttpClientModule } from '@angular/common/http';
import { Footer } from '../components/footer/footer';
import { CommonModule } from '@angular/common';
import { ToastComponent } from '../components/toast/toast';
import { ConfirmDialogComponent } from '../components/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, Nav, HttpClientModule, Footer, CommonModule, ToastComponent, ConfirmDialogComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  showFooter: boolean = true;

  constructor(private router: Router) {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.showFooter = event.url !== '/';
        this.showFooter = event.url !== '/home';
      }
    });
  }
}