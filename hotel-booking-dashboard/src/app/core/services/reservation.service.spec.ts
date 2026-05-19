import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { ReservationService } from './reservation.service';
import { Reservation, CreateReservationRequest, UpdateReservationRequest } from '../models/reservation.model';
import { environment } from '../../../environments/environment';

describe('ReservationService', () => {
  let service: ReservationService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.apiUrl}/api/reservations`;

  // ── Sample Data ───────────────────────────────────────
  const sampleReservations: Reservation[] = [
    {
      reservationId: 1,
      bookingRef: 'BK-10001',
      hotelName: 'The Grand Oberoi',
      guestName: 'Kunal',
      guestAge: 25,
      numberOfGuests: 2,
      checkInDate: '2026-05-10',
      checkOutDate: '2026-05-13',
      status: 'CONFIRMED'
    },
    {
      reservationId: 2,
      bookingRef: 'BK-10002',
      hotelName: 'Lemon Tree',
      guestName: 'Jammy',
      guestAge: 30,
      numberOfGuests: 1,
      checkInDate: '2026-05-15',
      checkOutDate: '2026-05-17',
      status: 'CANCELLED'
    }
  ];

  const createRequest: CreateReservationRequest = {
    hotelId: 1,
    guestName: 'Kunal',
    guestAge: 25,
    numberOfGuests: 2,
    checkInDate: '2026-05-10',
    checkOutDate: '2026-05-13'
  };

  const updateRequest: UpdateReservationRequest = {
    guestName: 'Kunal Updated',
    guestAge: 26,
    numberOfGuests: 3,
    checkInDate: '2026-05-10',
    checkOutDate: '2026-05-14'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        ReservationService,
        provideHttpClient(withFetch()),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(ReservationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  // ── getAll Tests ──────────────────────────────────────

  it('should fetch all reservations', () => {
    service.getAll().subscribe(reservations => {
      expect(reservations.length).toBe(2);
      expect(reservations[0].bookingRef).toBe('BK-10001');
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(sampleReservations);
  });

  it('should return empty list when no reservations exist', () => {
    service.getAll().subscribe(reservations => {
      expect(reservations.length).toBe(0);
    });

    const req = httpMock.expectOne(apiUrl);
    req.flush([]);
  });

  // ── getById Tests ─────────────────────────────────────

  it('should fetch reservation by id', () => {
    service.getById(1).subscribe(reservation => {
      expect(reservation.reservationId).toBe(1);
      expect(reservation.bookingRef).toBe('BK-10001');
      expect(reservation.status).toBe('CONFIRMED');
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(sampleReservations[0]);
  });

  it('should send GET request to correct url for reservation by id', () => {
    service.getById(2).subscribe();

    const req = httpMock.expectOne(`${apiUrl}/2`);
    expect(req.request.method).toBe('GET');
    req.flush(sampleReservations[1]);
  });

  // ── getMyBookings Tests ───────────────────────────────

  it('should fetch my bookings', () => {
    service.getMyBookings().subscribe(reservations => {
      expect(reservations.length).toBe(1);
      expect(reservations[0].status).toBe('CONFIRMED');
    });

    const req = httpMock.expectOne(`${apiUrl}/my-bookings`);
    expect(req.request.method).toBe('GET');
    req.flush([sampleReservations[0]]);
  });

  it('should return empty list when no bookings found', () => {
    service.getMyBookings().subscribe(reservations => {
      expect(reservations.length).toBe(0);
    });

    const req = httpMock.expectOne(`${apiUrl}/my-bookings`);
    req.flush([]);
  });

  // ── create Tests ──────────────────────────────────────

  it('should create a reservation with correct payload', () => {
    const mockResponse = {
      message: 'Reservation created successfully.',
      data: sampleReservations[0]
    };

    service.create(createRequest).subscribe(response => {
      expect(response.message).toBe('Reservation created successfully.');
      expect(response.data.bookingRef).toBe('BK-10001');
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(createRequest);
    req.flush(mockResponse);
  });

  it('should send POST request to correct url when creating', () => {
    service.create(createRequest).subscribe();

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  // ── update Tests ──────────────────────────────────────

  it('should update a reservation with correct payload', () => {
    const mockResponse = { message: 'Reservation updated successfully.' };

    service.update(1, updateRequest).subscribe(response => {
      expect(response.message).toBe('Reservation updated successfully.');
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updateRequest);
    req.flush(mockResponse);
  });

  it('should send PUT request to correct url when updating', () => {
    service.update(2, updateRequest).subscribe();

    const req = httpMock.expectOne(`${apiUrl}/2`);
    expect(req.request.method).toBe('PUT');
    req.flush({});
  });

  // ── cancel Tests ──────────────────────────────────────

  it('should cancel a reservation', () => {
    const mockResponse = { message: 'Reservation cancelled successfully.' };

    service.cancel(1).subscribe(response => {
      expect(response.message).toBe('Reservation cancelled successfully.');
    });

    const req = httpMock.expectOne(`${apiUrl}/1/cancel`);
    expect(req.request.method).toBe('PATCH');
    req.flush(mockResponse);
  });

  it('should send PATCH request to correct url when cancelling', () => {
    service.cancel(2).subscribe();

    const req = httpMock.expectOne(`${apiUrl}/2/cancel`);
    expect(req.request.method).toBe('PATCH');
    req.flush({});
  });

  it('should send empty body when cancelling', () => {
    service.cancel(1).subscribe();

    const req = httpMock.expectOne(`${apiUrl}/1/cancel`);
    expect(req.request.body).toEqual({});
    req.flush({});
  });
});