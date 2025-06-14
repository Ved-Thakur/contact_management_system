import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { ContactService } from 'src/app/services/contact.service';

@Component({
  selector: 'app-contact-form',
  templateUrl: './contact-form.component.html',
  styleUrls: ['./contact-form.component.css'],
})
export class ContactFormComponent implements OnInit, OnDestroy {
  contactForm!: FormGroup;
  contactId: string | null = null;
  isEditMode: boolean = false;
  errorMessage: string = '';
  private destroyed: Subject<void> = new Subject();

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
      this.contactsService
        .getContact(this.contactId!)
        .pipe(takeUntil(this.destroyed))
        .subscribe({
          next: (contact) => this.contactForm.patchValue(contact),
          error: () => alert('Failed to load contact'),
        });
    }
  }

  get name(): AbstractControl | null {
    return this.contactForm.get('name');
  }
  get email(): AbstractControl | null {
    return this.contactForm.get('email');
  }
  get phone(): AbstractControl | null {
    return this.contactForm.get('phone');
  }
  get address(): AbstractControl | null {
    return this.contactForm.get('address');
  }

  onSubmit(): void {
    if (this.contactForm.invalid) return;
    this.errorMessage = '';

    const contactData = this.contactForm.value;

    if (this.isEditMode) {
      this.contactsService
        .updateContact(this.contactId!, contactData)
        .pipe(takeUntil(this.destroyed))
        .subscribe({
          next: () => this.router.navigate(['/']),
          error: (err) => {
            this.errorMessage = err.error;
          },
        });
    } else {
      this.contactsService
        .createContact(contactData)
        .pipe(takeUntil(this.destroyed))
        .subscribe({
          next: () => this.router.navigate(['/']),
          error: (err) => {
            this.errorMessage = err.error;
          },
        });
    }
  }

  onCancel(): void {
    this.router.navigate(['/contacts']);
  }

  ngOnDestroy(): void {
    this.destroyed.next();
    this.destroyed.complete();
  }
}
