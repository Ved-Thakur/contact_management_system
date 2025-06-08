import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Contact, ContactFormData } from '../models/contact';

@Injectable({ providedIn: 'root' })
export class ContactService {
  private readonly API_URL = 'https://localhost:7182/api/contact';

  constructor(private http: HttpClient) {}

  getContacts(): Observable<Contact[]> {
    return this.http.get<Contact[]>(this.API_URL);
  }

  getContact(id: string): Observable<Contact> {
    return this.http.get<Contact>(`${this.API_URL}/${id}`);
  }

  createContact(contact: ContactFormData): Observable<Contact> {
    return this.http.post<Contact>(this.API_URL, contact);
  }

  updateContact(id: string, contact: ContactFormData): Observable<Object> {
    return this.http.put(`${this.API_URL}/${id}`, contact);
  }

  deleteContact(id: string): Observable<Object> {
    return this.http.delete(`${this.API_URL}/${id}`);
  }
}
