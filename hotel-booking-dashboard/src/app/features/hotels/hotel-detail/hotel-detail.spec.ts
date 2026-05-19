import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HotelDetail } from './hotel-detail';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { RouterModule } from '@angular/router';

describe('HotelDetail', () => {
  let component: HotelDetail;
  let fixture: ComponentFixture<HotelDetail>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HotelDetail,
        RouterModule.forRoot([])
      ],
      providers: [
        provideHttpClient(withFetch()),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HotelDetail);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
