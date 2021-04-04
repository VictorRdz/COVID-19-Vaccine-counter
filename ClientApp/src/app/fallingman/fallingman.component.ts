import { Component, OnInit, HostBinding } from '@angular/core';

@Component({
  selector: 'app-fallingman',
  templateUrl: './fallingman.component.html',
  styleUrls: ['./fallingman.component.scss']
})
export class FallingmanComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

  getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min)) + min;
  }

  imageNumber = this.getRandomInt(1, 9);

  startPositionVal = this.getRandomInt(10, 90);
  rotationStartVal = this.getRandomInt(45, 315);

  @HostBinding('style.--target-width')
  private targetWidth: string = this.getRandomInt(7, 9) + 'vw';

  @HostBinding('style.--duration-random')
  private durationRandom: string = this.getRandomInt(2, 5) + 's';

  @HostBinding('style.--start-position')
  private startPosition: string = this.startPositionVal + "%";
  @HostBinding('style.--end-position')
  private endPosition: string = this.startPositionVal + this.getRandomInt(-10, 10) + "%";

  @HostBinding('style.--rotation-start')
  private rotationStart: string = this.rotationStartVal + "deg";
  @HostBinding('style.--rotation-end')
  private rotationEnd: string = this.rotationStartVal + + this.getRandomInt(-45, 45) + "deg";
}