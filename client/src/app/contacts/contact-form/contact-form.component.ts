import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ContactService } from 'src/app/services/contact.service';

@Component({
  selector: 'app-contact-form',
  templateUrl: './contact-form.component.html',
  styleUrls: ['./contact-form.component.css'],
})
export class ContactFormComponent implements OnInit {
  contactForm!: FormGroup;
  contactId: string | null = null;
  isEditMode = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private contactsService: ContactService
  ) {}

  ngOnInit(): void {
    this.contactId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.contactId;

    this.contactForm = this.fb.group({
      name: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(20),
          Validators.pattern(/^[a-zA-Z\s]+$/),
        ],
      ],
      email: [
        '',
        [
          Validators.required,
          Validators.email,
          Validators.minLength(3),
          Validators.maxLength(50),
        ],
      ],
      phone: ['', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
      address: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(100),
          Validators.pattern(/^[a-zA-Z0-9\s,.-]+$/),
        ],
      ],
    });

    if (this.isEditMode) {
      this.contactsService.getContact(this.contactId!).subscribe({
        next: (contact) => this.contactForm.patchValue(contact),
        error: () => alert('Failed to load contact'),
      });
    }
  }

  // Getter methods for easy template access
  get name() {
    return this.contactForm.get('name');
  }
  get email() {
    return this.contactForm.get('email');
  }
  get phone() {
    return this.contactForm.get('phone');
  }
  get address() {
    return this.contactForm.get('address');
  }

  onSubmit() {
    if (this.contactForm.invalid) return;

    const contactData = this.contactForm.value;

    if (this.isEditMode) {
      this.contactsService
        .updateContact(this.contactId!, contactData)
        .subscribe({
          next: () => this.router.navigate(['/']),
          error: () => alert('Update failed'),
        });
    } else {
      this.contactsService.createContact(contactData).subscribe({
        next: () => this.router.navigate(['/']),
        error: () => alert('Add failed'),
      });
    }
  }

  onCancel() {
    this.router.navigate(['/contacts']);
  }
}
