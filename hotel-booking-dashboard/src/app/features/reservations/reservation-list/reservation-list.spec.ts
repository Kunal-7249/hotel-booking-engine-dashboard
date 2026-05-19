import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReservationList } from './reservation-list';
import { RouterModule } from '@angular/router';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('ReservationList', () => {
  let component: ReservationList;
  let fixture: ComponentFixture<ReservationList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ReservationList,
        RouterModule.forRoot([])
      ],
      providers: [
        provideHttpClient(withFetch()),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ReservationList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
