import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { interval } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Display } from 'src/models/display';

@Component({
  selector: 'app-display',
  templateUrl: './display.component.html',
  styleUrls: ['./display.component.css']
})
export class DisplayComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.updateDisplay();
  }

  ngOnDestroy(): void {
    this.updater?.unsubscribe();
  }

  
  @Input() title: string;
  @Input("display") nextDisplay: Display;
  
  updater;
  display: Display;
  imageUrl = "assets/vaccines/vaccine_1.png";
  iconUrl = "assets/earth.png";

  updateDisplay() {
    if(this.display == undefined) {
      this.display = this.nextDisplay;
    }
    else {
      let totalFrames = environment.fps * environment.updateTime;
      let increment = (this.nextDisplay.value - this.display.value) / totalFrames;
      
      this.updater?.unsubscribe();
      this.updater = interval(1000 / environment.fps).subscribe(i => {
        if(i >= totalFrames) {
          // Small increment on http delay
          this.display.value += increment * 0.5;
        }
        else {
          this.display.value += increment * this.getRandomArbitrary(0.50, 1.51);
        }
      });
    }
  }

  getRandomArbitrary(min, max) {
    return Math.random() * (max - min) + min;
  }

}
