import { Component, ComponentFactoryResolver, ViewContainerRef } from '@angular/core';
import { interval, Observable, Subscription, timer } from 'rxjs';
import { mergeMap } from 'rxjs/operators'
import { Covid } from 'src/models/covid';
import { CoviddataService } from './coviddata.service';
import { FallingmanComponent } from './fallingman/fallingman.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(
    private vf:ViewContainerRef,
    private componentFactoryResolver:ComponentFactoryResolver,
    private coviddateService:CoviddataService) { }
  
  zone = "world";
  displayField = "vaccined";

  covidData: Covid;
  previousCovidData: Covid;
  displayValue: number = 0;
  displayIterationsPerSecond = 50;
  
  updateTime = 5; // in seconds
  maxDisplayFallingman = 5;

  displaySubscription = Subscription.EMPTY;
  quantityOffset = 0; // offset for time between http get

  displayError = 0;
  bigError = false;

  ngOnInit() {
    this.updateApp(this.zone, this.displayField);
  }

  updateApp(zone, displayField) {
    interval(this.updateTime * 1000).pipe(
      mergeMap(() => {
        let intervalCovidData = this.getIntervalCovidData();
        this.resetDisplayIfError();
        this.displaySubscription.unsubscribe();
        this.updateDisplayValue(intervalCovidData[displayField], this.updateTime);
        this.displayFallingman(intervalCovidData[displayField], this.updateTime);
        if(this.covidData != undefined) {
          this.displayError = this.covidData.error / 100;
        }
        if(Math.abs(this.displayError) >= 5) {
          this.bigError = true
        }
        else {
          this.bigError = false;
        }

        return this.coviddateService.getData(zone);
      })
    )
    .subscribe(data => {
      // Get previous data before update
      if(this.covidData != undefined){
        this.previousCovidData = this.covidData;
      }
      return this.covidData = data
    });
  }

  resetDisplayIfError() {
    if(this.previousCovidData != undefined && this.covidData != undefined && this.displayValue != 0) {
      let error = Math.abs(100 - (100 / this.previousCovidData[this.displayField] * this.displayValue));
      // console.log("Error entre data y display: " + error + "%");
      if(error > 1 || this.displayValue >= this.covidData[this.displayField]) {
        this.displayValue = this.previousCovidData[this.displayField];
      }
    }

  }

  getIntervalCovidData(): Covid {
    let intervalCovidData: Covid = {confirmed : 0, deaths : 0, recovered : 0, vaccined : 0, error : 0};
    let keys = ["confirmed", "deaths", "recovered", "vaccined", "error"];
    if(this.covidData != undefined && this.previousCovidData != undefined) {
      keys.forEach(key => {
        intervalCovidData[key] = this.covidData[key] - this.previousCovidData[key];
        //if (intervalCovidData[key] < 0) { intervalCovidData[key] = 0 }
      });
    }
    return intervalCovidData;
  }
  

  displayFallingman(_quantity, time) {
    if (_quantity <= 0 || time <= 0 ) { return };

    let quantity = _quantity;
    let fallingmanRatio = _quantity / time;
    // If is more than maxDisplay, then reduce
    if(fallingmanRatio > this.maxDisplayFallingman) {
      quantity = Math.round(this.maxDisplayFallingman * time);
    }

    let msTime = time * 1000;
    let msInterval = Math.round(msTime / quantity);

    let aux = 0;

    let subscription = interval(msInterval).pipe(
      mergeMap(() => {
        this.createFallingman();
        aux++;
        // Unsubscribe when all fallingmen were displayed
        if(aux >= quantity) {
          subscription.unsubscribe();
        }
        return new Observable;
      })
    ).subscribe();
  }

  updateDisplayValue(_quantity, _time) {
    if(this.covidData != undefined && this.previousCovidData != undefined && this.displayValue == 0) {
      this.displayValue = this.previousCovidData[this.displayField];
    }
    
    _quantity += this.quantityOffset;
    if (_quantity <= 0 || _time <= 0 ) { return };

    let speedratio = this.getDisplaySpeedRatio(_quantity, _time);
    let addedValue = speedratio[0];
    let quantity = speedratio[1];
    

    let msTime = _time * 1000;
    let msInterval = Math.floor(msTime / quantity);
    let aux = 0;
    
    // console.log(1000/msInterval + " iteraciones por segundo");
    // console.log("Margen de error: " + ((this.displayValue - this.previousCovidData[this.displayField]) - this.quantityOffset));
    this.displaySubscription = interval(msInterval).pipe(
      mergeMap(() => {
        this.displayValue += addedValue;
        aux++;
        // Unsubscribe when all fallingmen were displayed
        if(aux >= quantity) {
          this.quantityOffset = this.previousCovidData[this.displayField] - this.displayValue;
        }
        return new Observable;
      })
    ).subscribe();
  }

  getDisplaySpeedRatio (_quantity, _time) {
    let displayRatio = _quantity / _time;
    let addedValue;
    let newQuantity;
    // 1000/s
    if(displayRatio >= this.displayIterationsPerSecond) {
      addedValue = Math.round(displayRatio/this.displayIterationsPerSecond);
      newQuantity = Math.round(_quantity / addedValue);
    }
    else if(displayRatio > 0 ) {
      addedValue = 1;
      newQuantity = _quantity;
    }
    return([addedValue, newQuantity])
    
  }


  createFallingman() {
    let resolver = this.componentFactoryResolver.resolveComponentFactory(FallingmanComponent);
    let componentFactory = this.vf.createComponent(resolver);
    setTimeout(()=>{
      componentFactory.destroy();
    }, 5000);
  }
  
}