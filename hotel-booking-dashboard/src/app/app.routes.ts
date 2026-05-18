import { Routes } from '@angular/router';
import { authGuard, adminGuard, externalUserGuard } from './core/guards/auth.guard';
import { HotelList } from './features/hotels/hotel-list/hotel-list';
import { HotelDetail } from './features/hotels/hotel-detail/hotel-detail';
import { ReservationList } from './features/reservations/reservation-list/reservation-list';
import { ModifyForm } from './features/reservations/modify-form/modify-form';
import { BookingForm } from './features/reservations/booking-form/booking-form';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { MyBookings } from './features/reservations/my-bookings/my-bookings';

export const routes: Routes = [
  { path: '', redirectTo: 'hotels', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'hotels', component: HotelList },
  { path: 'hotels/:id', component: HotelDetail },
  {
    path: 'hotels/:id/book',
    component: BookingForm,
    canActivate: [authGuard]          
  },
  {
    path: 'reservations',
    component: ReservationList,
    canActivate: [authGuard, adminGuard]  
  },
  {
    path: 'reservations/:id/modify',
    component: ModifyForm,
    canActivate: [authGuard, adminGuard] 
  },
  {
    path: 'my-bookings',
    component: MyBookings,
    canActivate: [authGuard, externalUserGuard]  
  },
  { path: '**', redirectTo: 'hotels' }
];
