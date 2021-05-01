import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FallingVaccineComponent } from './falling-vaccine.component';

describe('FallingmanComponent', () => {
  let component: FallingVaccineComponent;
  let fixture: ComponentFixture<FallingVaccineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FallingVaccineComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FallingVaccineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
