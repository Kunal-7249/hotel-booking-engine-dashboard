import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Hotel, HotelDetail } from '../models/hotel.model';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/paged-result.model';

@Injectable({ providedIn: 'root' })
export class HotelService {
  private apiUrl = `${environment.apiUrl}/api/hotels`;

  constructor(private http: HttpClient) {}

  getAll(city?: string, page: number = 1, pageSize: number = 6): Observable<PagedResult<Hotel>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (city) params = params.set('city', city);

    return this.http.get<PagedResult<Hotel>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<HotelDetail> {
    return this.http.get<HotelDetail>(`${this.apiUrl}/${id}`);
  }
}
