import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Reservation,
  CreateReservationRequest,
  UpdateReservationRequest
} from '../models/reservation.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ReservationService {
  private apiUrl = `${environment.apiUrl}/api/reservations`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(this.apiUrl);
  }

  getById(id: number): Observable<Reservation> {
    return this.http.get<Reservation>(`${this.apiUrl}/${id}`);
  }

  getMyBookings(): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(`${this.apiUrl}/my-bookings`);
  }

  create(data: CreateReservationRequest): Observable<any> {
    return this.http.post(this.apiUrl, data);
  }

  update(id: number, data: UpdateReservationRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, data);
  }

  cancel(id: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}/cancel`, {});
  }
}