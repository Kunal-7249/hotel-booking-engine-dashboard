import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModifyForm } from './modify-form';
import { provideRouter, RouterModule } from '@angular/router';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('ModifyForm', () => {
  let component: ModifyForm;
  let fixture: ComponentFixture<ModifyForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ModifyForm,
        RouterModule.forRoot([])
      ],
      providers: [
        provideRouter([]),
        provideHttpClient(withFetch()),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ModifyForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
