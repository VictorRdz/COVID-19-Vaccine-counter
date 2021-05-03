import { Component } from '@angular/core';
import { interval } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Display } from 'src/models/display';
import { DataService } from './data.service';
import { DisplayComponent } from './web/display/display.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(private _dataService: DataService) { }
  
  total: Display;
  peopleFully: Display;

  totalUpdater;
  peopleFullyUpdater;
  obs;

  ngOnInit() {
    this.obs = window.location.pathname == "/obs";

    // Initialize counter
    this._dataService.getData("total").subscribe(value => this.total = value);
    this._dataService.getData("people-fully").subscribe(value => this.peopleFully = value);

    // Temporal increment until updateTime
    let temporalUpdater = interval(1000).subscribe(i => {
      this._dataService.getData("total").subscribe(display => this.total = 
        {value: this.total.value + (display.value - this.total.value) * environment.updateTime * 0.5, dataError: display.dataError, predictionError: display.predictionError}
      );

      this._dataService.getData("people-fully").subscribe(display => this.peopleFully = 
        {value: this.peopleFully.value + (display.value - this.peopleFully.value) * environment.updateTime * 0.5, dataError: display.dataError, predictionError: display.predictionError}
      );
      temporalUpdater.unsubscribe();
    });

    // Update counter with updateTime
    this.totalUpdater = interval(1000 * environment.updateTime).subscribe(data => {
      return this._dataService.getData("total").subscribe(value => this.total = value);
    });

    this.peopleFullyUpdater = interval(1000 * environment.updateTime).subscribe(data => {
      return this._dataService.getData("people-fully").subscribe(value => this.peopleFully = value);
    });

  }

  ngOnDestroy(): void {
    this.totalUpdater.unsubscribe();
    this.peopleFullyUpdater.unsubscribe();
  }



}