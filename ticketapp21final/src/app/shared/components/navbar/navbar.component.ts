import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  auth = inject(AuthService);

  initials(): string {
    const n = this.auth.currentUser()?.displayName ?? '';
    return n.split(' ').map(x => x[0]).slice(0,2).join('').toUpperCase() || 'U';
  }
}
