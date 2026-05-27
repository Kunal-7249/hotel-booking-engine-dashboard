import { Component, inject, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { DatePipe } from '@angular/common';
import { ReservationService } from '../../../core/services/reservation.service';
import { Reservation } from '../../../core/models/reservation.model';
import { PagedResult } from '../../../core/models/paged-result.model';

@Component({
  selector: 'app-my-bookings',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatPaginatorModule,   // ← add
    DatePipe
  ],
  templateUrl: './my-bookings.html',
  styleUrl: './my-bookings.scss',
})
export class MyBookings implements OnInit {
  private reservationService = inject(ReservationService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  reservations = signal<Reservation[]>([]);
  totalCount = signal(0);
  isLoading = signal(false);
  currentPage = signal(0);
  pageSize = signal(10);
  displayedColumns = ['bookingRef', 'hotelName', 'checkInDate', 'checkOutDate', 'status'];

  ngOnInit(): void {
    this.loadMyBookings();
  }

  loadMyBookings(): void {
    this.isLoading.set(true);
    this.reservationService.getMyBookings(
      this.currentPage() + 1,
      this.pageSize()
    ).subscribe({
      next: (result: PagedResult<Reservation>) => {
        this.reservations.set(result.items);
        this.totalCount.set(result.totalCount);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.snackBar.open('Failed to load bookings.', 'Close', { duration: 3000 });
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadMyBookings();
  }

  bookAnother(): void {
    this.router.navigate(['/hotels']);
  }
}
