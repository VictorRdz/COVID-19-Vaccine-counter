import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FallingmanComponent } from './fallingman.component';

describe('FallingmanComponent', () => {
  let component: FallingmanComponent;
  let fixture: ComponentFixture<FallingmanComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FallingmanComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FallingmanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
