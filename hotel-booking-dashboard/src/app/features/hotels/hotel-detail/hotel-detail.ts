
import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DecimalPipe } from '@angular/common';
import { HotelService } from '../../../core/services/hotel.service';
import { HotelDetail as HotelDetailModel} from '../../../core/models/hotel.model';

@Component({
  selector: 'app-hotel-detail',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    DecimalPipe
  ],
  templateUrl: './hotel-detail.html',
  styleUrl: './hotel-detail.scss',
})

export class HotelDetail implements OnInit {
  private hotelService = inject(HotelService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  hotel = signal<HotelDetailModel | null>(null);
  isLoading = signal(false);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.isLoading.set(true);
    this.hotelService.getById(id).subscribe({
      next: (data) => {
        this.hotel.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  getStars(rating: number): string {
    return '★'.repeat(rating) + '☆'.repeat(5 - rating);
  }

  bookHotel(): void {
    this.router.navigate(['/hotels', this.hotel()?.hotelId, 'book']);
  }

  goBack(): void {
    this.router.navigate(['/hotels']);
  }
}
