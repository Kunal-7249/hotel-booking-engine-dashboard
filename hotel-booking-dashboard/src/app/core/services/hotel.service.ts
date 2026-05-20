import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Hotel, HotelDetail } from '../models/hotel.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class HotelService {
  private apiUrl = `${environment.apiUrl}/api/hotels`;

  constructor(private http: HttpClient) {}

  getAll(city?: string): Observable<Hotel[]> {
    const params = city ? new HttpParams().set('city', city) : undefined;
    return this.http.get<Hotel[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<HotelDetail> {
    return this.http.get<HotelDetail>(`${this.apiUrl}/${id}`);
  }
}