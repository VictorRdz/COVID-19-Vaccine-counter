import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WebComponent } from './web.component';
import { DisplayComponent } from './display/display.component';
import { FallingVaccineComponent } from './falling-vaccine/falling-vaccine.component';



@NgModule({
  declarations: [WebComponent, DisplayComponent, FallingVaccineComponent],
  imports: [
    CommonModule
  ],
  exports: [WebComponent],
  bootstrap: [WebComponent]
})
export class WebModule { }
