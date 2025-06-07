import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  ContactFormData,
  ContactService,
} from 'src/app/services/contact.service';

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
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', Validators.required],
      address: ['', Validators.required],
    });

    if (this.isEditMode) {
      this.contactsService.getContact(this.contactId!).subscribe({
        next: (contact) => this.contactForm.patchValue(contact),
        error: () => alert('Failed to load contact'),
      });
    }
  }

  onSubmit() {
    if (this.contactForm.invalid) return;

    const contactData: ContactFormData = this.contactForm.value;

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
