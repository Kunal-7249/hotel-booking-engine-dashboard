import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { HotelService } from './hotel.service';
import { Hotel, HotelDetail } from '../models/hotel.model';

describe('HotelService', () => {
  let service: HotelService;
  let httpMock: HttpTestingController;

  const apiUrl = 'http://localhost:5018/api/hotels';

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
        provideHttpClient(withFetch()),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(HotelService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should fetch all hotels without city filter', () => {
  service.getAll().subscribe(result => {
    expect(result.items.length).toBe(2);
    expect(result.items[0].name).toBe('The Grand Oberoi');
  });

  const req = httpMock.expectOne(`${apiUrl}?page=1&pageSize=6`);
  expect(req.request.method).toBe('GET');
  req.flush({
    items: sampleHotels,
    totalCount: 2,
    page: 1,
    pageSize: 6,
    totalPages: 1,
    hasNextPage: false,
    hasPreviousPage: false
  });
});

it('should fetch hotels with city filter', () => {
  service.getAll('Mumbai').subscribe(result => {
    expect(result.items.length).toBe(1);
    expect(result.items[0].city).toBe('Mumbai');
  });

  const req = httpMock.expectOne(`${apiUrl}?page=1&pageSize=6&city=Mumbai`);
  expect(req.request.method).toBe('GET');
  req.flush({
    items: [sampleHotels[0]],
    totalCount: 1,
    page: 1,
    pageSize: 6,
    totalPages: 1,
    hasNextPage: false,
    hasPreviousPage: false
  });
});

it('should return empty list when no hotels found', () => {
  service.getAll().subscribe(result => {
    expect(result.items.length).toBe(0);
    expect(result.totalCount).toBe(0);
  });

  const req = httpMock.expectOne(`${apiUrl}?page=1&pageSize=6`);
  req.flush({
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 6,
    totalPages: 0,
    hasNextPage: false,
    hasPreviousPage: false
  });
});

  it('should fetch hotel detail by id', () => {
    service.getById(1).subscribe(hotel => {
      expect(hotel.hotelId).toBe(1);
      expect(hotel.name).toBe('The Grand Oberoi');
      expect(hotel.isAvailable).toBeTruthy();
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
