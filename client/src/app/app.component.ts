import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  constructor(private authService: AuthService) {}
  isLoggedIn: boolean;
  ngOnInit(): void {
    this.isLoggedIn = this.authService.isLoggedIn();
  }

  title = 'client';
}
