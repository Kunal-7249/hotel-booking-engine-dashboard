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
  pageSize = signal(8);

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

  getHotelImage(id: number): string {
    const images: Record<number, string> = {
      1: 'https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=400&h=200&fit=crop',
      2: 'https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=400&h=200&fit=crop',
      3: 'https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=400&h=200&fit=crop',
      4: 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=400&h=200&fit=crop',
      5: 'https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=400&h=200&fit=crop',
      6: 'https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=400&h=200&fit=crop',
      7: 'https://images.unsplash.com/photo-1564501049412-61c2a3083791?w=400&h=200&fit=crop',
      8: 'https://images.unsplash.com/photo-1553653924-39b70295f8da?w=400&h=200&fit=crop',
      9: 'https://images.unsplash.com/photo-1506059612708-99d6c258160e?w=400&h=200&fit=crop',
      10: 'https://images.unsplash.com/photo-1455587734955-081b22074882?w=400&h=200&fit=crop',
      11: 'https://images.unsplash.com/photo-1496417263034-38ec4f0b665a?w=400&h=200&fit=crop',
      12: 'https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=400&h=200&fit=crop',
    };
    return images[id] ?? images[1];
  }
}
