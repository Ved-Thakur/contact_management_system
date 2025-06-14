import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    const isLoggedIn: boolean = this.authService.isLoggedIn();

    if (!isLoggedIn) {
      this.router.navigate(['/auth/login']);
      return false;
    }

    return true;
  }
}
