import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Reservation, CreateReservationRequest, UpdateReservationRequest } from '../models/reservation.model';
import { PagedResult } from '../models/paged-result.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ReservationService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/reservations`;

  getAll(page: number = 1, pageSize: number = 10): Observable<PagedResult<Reservation>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PagedResult<Reservation>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<Reservation> {
    return this.http.get<Reservation>(`${this.apiUrl}/${id}`);
  }

  getMyBookings(page: number = 1, pageSize: number = 10): Observable<PagedResult<Reservation>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PagedResult<Reservation>>(`${this.apiUrl}/my-bookings`, { params });
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
