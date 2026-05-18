import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HotelService } from '../../../core/services/hotel.service';
import { ReservationService } from '../../../core/services/reservation.service';
import { Hotel } from '../../../core/models/hotel.model';


// Cross-field validator: checkout must be after checkin
function checkoutAfterCheckin(control: AbstractControl): ValidationErrors | null {
  const checkIn = control.get('checkInDate')?.value;
  const checkOut = control.get('checkOutDate')?.value;
  if (checkIn && checkOut && new Date(checkOut) <= new Date(checkIn)) {
    return { checkoutBeforeCheckin: true };
  }
  return null;
}

@Component({
  selector: 'app-booking-form',
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './booking-form.html',
  styleUrl: './booking-form.scss'
})

export class BookingForm implements OnInit{
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private hotelService = inject(HotelService);
  private reservationService = inject(ReservationService);

  hotel = signal<Hotel | null>(null);
  isSubmitting = signal(false);
  errorMessage = signal('');
  today = new Date();

  form: FormGroup = this.fb.group({
    guestName: ['', [Validators.required]],
    guestAge: [null, [Validators.required, Validators.min(18)]],
    numberOfGuests: [null, [Validators.required, Validators.min(1)]],
    checkInDate: [null, [Validators.required]],
    checkOutDate: [null, [Validators.required]]
  }, { validators: checkoutAfterCheckin });

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.hotelService.getById(id).subscribe({
      next: (data) => this.hotel.set(data)
    });
  }

  get f() {
    return this.form.controls;
  }

  onSubmit(): void {
    this.form.markAllAsTouched();  

    if (this.form.invalid) {
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    const payload = {
      hotelId: this.hotel()!.hotelId,
      guestName: this.f['guestName'].value,
      guestAge: this.f['guestAge'].value,
      numberOfGuests: this.f['numberOfGuests'].value,
      checkInDate: this.formatDate(this.f['checkInDate'].value),
      checkOutDate: this.formatDate(this.f['checkOutDate'].value)
    };

    this.reservationService.create(payload).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.router.navigate(['/reservations'], {
          queryParams: { success: 'Reservation created successfully!' }
        });
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err.error?.message || 'Something went wrong.');
      }
    });
  }

  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  goBack(): void {
    this.router.navigate(['/hotels', this.hotel()?.hotelId]);
  }
}
