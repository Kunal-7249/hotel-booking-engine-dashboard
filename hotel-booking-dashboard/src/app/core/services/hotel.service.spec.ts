import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { HotelService } from './hotel.service';
import { Hotel, HotelDetail } from '../models/hotel.model';

describe('HotelService', () => {
  let service: HotelService;
  let httpMock: HttpTestingController;
  const apiUrl = 'https://localhost:5018/api/hotels';

  const sampleHotels: Hotel[] = [
    { hotelId: 1, name: 'The Grand Oberoi', city: 'Mumbai', starRating: 5, pricePerNight: 12000 },
    { hotelId: 2, name: 'Lemon Tree', city: 'Pune', starRating: 4, pricePerNight: 5500 }
  ];

  const sampleHotelDetail: HotelDetail = {
    hotelId: 1,
    name: 'The Grand Oberoi',
    city: 'Mumbai',
    starRating: 5,
    pricePerNight: 12000,
    description: 'Luxury hotel in Mumbai',
    isAvailable: true
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        HotelService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(HotelService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();  // ← ensures no unexpected HTTP calls
  });

  // ── getAll Tests ──────────────────────────────────────

  it('should fetch all hotels without city filter', () => {
    service.getAll().subscribe(hotels => {
      expect(hotels.length).toBe(2);
      expect(hotels[0].name).toBe('The Grand Oberoi');
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(sampleHotels);
  });

  it('should fetch hotels with city filter', () => {
    service.getAll('Mumbai').subscribe(hotels => {
      expect(hotels.length).toBe(1);
      expect(hotels[0].city).toBe('Mumbai');
    });

    const req = httpMock.expectOne(`${apiUrl}?city=Mumbai`);
    expect(req.request.method).toBe('GET');
    req.flush([sampleHotels[0]]);
  });

  it('should return empty list when no hotels found', () => {
    service.getAll().subscribe(hotels => {
      expect(hotels.length).toBe(0);
    });

    const req = httpMock.expectOne(apiUrl);
    req.flush([]);
  });

  // ── getById Tests ─────────────────────────────────────

  it('should fetch hotel detail by id', () => {
    service.getById(1).subscribe(hotel => {
      expect(hotel.hotelId).toBe(1);
      expect(hotel.name).toBe('The Grand Oberoi');
      expect(hotel.isAvailable).toBe(true);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(sampleHotelDetail);
  });

  it('should send GET request to correct URL for hotel detail', () => {
    service.getById(2).subscribe();

    const req = httpMock.expectOne(`${apiUrl}/2`);
    expect(req.request.method).toBe('GET');
    req.flush(sampleHotelDetail);
  });
});