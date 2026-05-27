import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { DatePipe } from '@angular/common';
import { ReservationService } from '../../../core/services/reservation.service';
import { Reservation } from '../../../core/models/reservation.model';
import { PagedResult } from '../../../core/models/paged-result.model';
import { ConfirmDialog } from '../../../shared/components/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-reservation-list',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatSnackBarModule,
    MatPaginatorModule,   // ← add
    DatePipe
  ],
  templateUrl: './reservation-list.html',
  styleUrl: './reservation-list.scss'
})
export class ReservationList implements OnInit {
  private reservationService = inject(ReservationService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  reservations = signal<Reservation[]>([]);
  totalCount = signal(0);
  isLoading = signal(false);
  currentPage = signal(0);
  pageSize = signal(10);
  displayedColumns = ['bookingRef', 'hotelName', 'guestName', 'checkInDate', 'checkOutDate', 'status', 'actions'];

  ngOnInit(): void {
    const success = this.route.snapshot.queryParamMap.get('success');
    if (success) {
      this.snackBar.open(success, 'Close', { duration: 4000, panelClass: 'snack-success' });
    }
    this.loadReservations();
  }

  loadReservations(): void {
    this.isLoading.set(true);
    this.reservationService.getAll(
      this.currentPage() + 1,
      this.pageSize()
    ).subscribe({
      next: (result: PagedResult<Reservation>) => {
        this.reservations.set(result.items);
        this.totalCount.set(result.totalCount);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadReservations();
  }

  modifyReservation(id: number): void {
    this.router.navigate(['/reservations', id, 'modify']);
  }

  cancelReservation(reservation: Reservation): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '400px',
      data: { bookingRef: reservation.bookingRef }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.reservationService.cancel(reservation.reservationId).subscribe({
          next: () => {
            this.snackBar.open('Reservation cancelled successfully.', 'Close', { duration: 3000 });
            this.loadReservations();
          },
          error: (err) => {
            this.snackBar.open(err.error?.message || 'Cancellation failed.', 'Close', { duration: 3000 });
          }
        });
      }
    });
  }
}
