import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { WebModule } from './web/web.module';
import { ObsModule } from './obs/obs.module';

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    ObsModule,
    WebModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
