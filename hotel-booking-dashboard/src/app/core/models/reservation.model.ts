export interface Reservation {
  reservationId: number;
  bookingRef: string;
  hotelName: string;
  guestName: string;
  guestAge: number;        
  numberOfGuests: number;
  checkInDate: string;
  checkOutDate: string;
  status: 'CONFIRMED' | 'CANCELLED';
}

export interface CreateReservationRequest {
  hotelId: number;
  guestName: string;
  guestAge: number;
  numberOfGuests: number;
  checkInDate: string;
  checkOutDate: string;
}

export interface UpdateReservationRequest {
  guestName: string;
  guestAge: number;
  numberOfGuests: number;
  checkInDate: string;
  checkOutDate: string;
}