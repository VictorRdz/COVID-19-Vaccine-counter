import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DisplayComponent } from './display/display.component';
import { FallingVaccineComponent } from './falling-vaccine/falling-vaccine.component';
import { ObsComponent } from './obs.component';



@NgModule({
  declarations: [ObsComponent, DisplayComponent, FallingVaccineComponent],
  imports: [
    CommonModule
  ],
  exports: [ObsComponent],
  bootstrap: [ObsComponent]
})
export class ObsModule { }
