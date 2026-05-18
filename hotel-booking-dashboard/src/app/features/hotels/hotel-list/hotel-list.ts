import { Component, inject, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { HotelService } from '../../../core/services/hotel.service';
import { Hotel } from '../../../core/models/hotel.model';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-hotel-list',
  imports: [
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
    MatIconModule,
    DecimalPipe
  ],
  templateUrl: './hotel-list.html',
  styleUrl: './hotel-list.scss',
})

export class HotelList implements OnInit {
  private hotelService = inject(HotelService);
  private router = inject(Router);

  hotels = signal<Hotel[]>([]);
  isLoading = signal(false);
  cityFilter = signal('');

  ngOnInit(): void {
    this.loadHotels();
  }

  loadHotels(): void {
    this.isLoading.set(true);
    this.hotelService.getAll(this.cityFilter() || undefined).subscribe({
      next: (data) => {
        this.hotels.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  onFilterChange(): void {
    this.loadHotels();
  }

  viewDetail(id: number): void {
    this.router.navigate(['/hotels', id]);
  }

  bookHotel(id: number): void {
    this.router.navigate(['/hotels', id, 'book']);
  }

  getStars(rating: number): string {
    return '★'.repeat(rating) + '☆'.repeat(5 - rating);
  }
}
