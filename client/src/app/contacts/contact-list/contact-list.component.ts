import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { Contact, ContactService } from 'src/app/services/contact.service';

@Component({
  selector: 'app-contact-list',
  templateUrl: './contact-list.component.html',
  styleUrls: ['./contact-list.component.css'],
})
export class ContactListComponent implements OnInit, OnDestroy {
  contacts: Contact[] = [];
  private destroyed: Subject<void> = new Subject();

  constructor(
    private contactsService: ContactService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadContacts();
  }

  displayedColumns: string[] = ['name', 'email', 'phone', 'address', 'actions'];

  editContact(contact: Contact) {
    this.router.navigate(['contacts/edit', contact.id]);
  }

  addContact() {
    this.router.navigate(['contacts/add']);
  }

  logout() {
    this.authService.logout();
  }

  loadContacts() {
    this.contactsService
      .getContacts()
      .pipe(takeUntil(this.destroyed))
      .subscribe({
        next: (contacts) => {
          this.contacts = contacts;
          console.log(contacts);
        },
        error: (err) => console.error('Failed to load contacts', err),
      });
  }

  deleteContact(id: string) {
    this.contactsService
      .deleteContact(id)
      .pipe(takeUntil(this.destroyed))
      .subscribe({
        next: () => (this.contacts = this.contacts.filter((c) => c.id !== id)),
        error: (err) => console.error('Delete failed', err),
      });
  }

  ngOnDestroy(): void {
    this.destroyed.next();
    this.destroyed.complete();
  }
}
