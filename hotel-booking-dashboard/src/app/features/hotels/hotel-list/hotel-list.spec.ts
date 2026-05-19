import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HotelList } from './hotel-list';
import { RouterModule } from '@angular/router';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('HotelList', () => {
  let component: HotelList;
  let fixture: ComponentFixture<HotelList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HotelList,
        RouterModule.forRoot([])
      ],
      providers: [
        provideHttpClient(withFetch()),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HotelList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
