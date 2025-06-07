import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

export interface Contact {
  id: string;
  name: string;
  email: string;
  phone: string;
  address: string;
}

// models/contact-form.model.ts
export interface ContactFormData {
  name: string;
  email: string;
  phone: string;
  address: string;
}

@Injectable({ providedIn: 'root' })
export class ContactService {
  private readonly API_URL = 'https://localhost:7182/api/contact';

  constructor(private http: HttpClient) {}

  getContacts() {
    return this.http.get<Contact[]>(this.API_URL);
  }

  getContact(id: string) {
    return this.http.get<Contact>(`${this.API_URL}/${id}`);
  }

  createContact(contact: ContactFormData) {
    return this.http.post(this.API_URL, contact);
  }

  updateContact(id: string, contact: ContactFormData) {
    return this.http.put(`${this.API_URL}/${id}`, contact);
  }

  deleteContact(id: string) {
    return this.http.delete(`${this.API_URL}/${id}`);
  }
}
