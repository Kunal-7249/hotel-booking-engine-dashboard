import { Component, inject, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { DecimalPipe } from '@angular/common';
import { HotelService } from '../../../core/services/hotel.service';
import { Hotel } from '../../../core/models/hotel.model';
import { PagedResult } from '../../../core/models/paged-result.model';

@Component({
  selector: 'app-hotel-list',
  standalone: true,
  imports: [
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatPaginatorModule,
    DecimalPipe
  ],
  templateUrl: './hotel-list.html',
  styleUrl: './hotel-list.scss'
})
export class HotelList implements OnInit {
  private hotelService = inject(HotelService);
  private router = inject(Router);

  hotels = signal<Hotel[]>([]);
  totalCount = signal(0);
  isLoading = signal(false);
  cityFilter = signal('');
  currentPage = signal(0);
  pageSize = signal(6);

  ngOnInit(): void {
    this.loadHotels();
  }

  loadHotels(): void {
    this.isLoading.set(true);

    this.hotelService.getAll(
      this.cityFilter() || undefined,
      this.currentPage() + 1,
      this.pageSize()
    ).subscribe({
      next: (result: PagedResult<Hotel>) => {
        this.hotels.set(result.items);
        this.totalCount.set(result.totalCount);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  onFilterChange(): void {
    this.currentPage.set(0);
    this.loadHotels();
  }

  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
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
