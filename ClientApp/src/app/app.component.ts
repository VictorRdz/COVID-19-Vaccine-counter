import { Component } from '@angular/core';
import { interval } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Display } from 'src/models/display';
import { DataService } from './data.service';

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