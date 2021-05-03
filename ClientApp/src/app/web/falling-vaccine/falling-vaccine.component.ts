import { Component, OnInit, HostBinding } from '@angular/core';

@Component({
  selector: 'app-falling-vaccine',
  templateUrl: './falling-vaccine.component.html',
  styleUrls: ['./falling-vaccine.component.scss']
})
export class FallingVaccineComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
    setTimeout(() => {
      this.hide = true;
  }, this.duration * 1000);
  }

  getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min)) + min;
  }

  hide = false;
  imageNumber = this.getRandomInt(1, 9);
  startPositionVal = this.getRandomInt(0, 90);
  rotationStartVal = this.getRandomInt(45, 315);

  @HostBinding('style.--target-width')
  private targetWidth: string = this.getRandomInt(6, 9) + 'vw';

  public duration = this.getRandomInt(3, 5);
  @HostBinding('style.--duration-random')
  private durationRandom: string = this.duration + 's';

  @HostBinding('style.--start-position')
  private startPosition: string = this.startPositionVal + "%";
  @HostBinding('style.--end-position')
  private endPosition: string = this.startPositionVal + this.getRandomInt(-9, 10) + "%";

  @HostBinding('style.--rotation-start')
  private rotationStart: string = this.rotationStartVal + "deg";
  @HostBinding('style.--rotation-end')
  private rotationEnd: string = this.rotationStartVal + + this.getRandomInt(-45, 45) + "deg";
}