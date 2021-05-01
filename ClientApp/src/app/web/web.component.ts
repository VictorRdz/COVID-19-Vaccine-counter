import { Input, ViewContainerRef } from '@angular/core';
import { ComponentFactoryResolver } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { interval } from 'rxjs';
import { Display } from 'src/models/display';
import { FallingVaccineComponent } from './falling-vaccine/falling-vaccine.component';

@Component({
  selector: 'app-web',
  templateUrl: './web.component.html',
  styleUrls: ['./web.component.css']
})
export class WebComponent implements OnInit {

  constructor(
    private componentFactoryResolver:ComponentFactoryResolver,
    private vf:ViewContainerRef) { }

  ngOnInit(): void {
    this.vaccineDisplay = interval(500).subscribe(data => this.createFallingman());
  }

  @Input() peopleFully: Display;
  @Input() total: Display;
  vaccineDisplay;

  createFallingman() {
    let resolver = this.componentFactoryResolver.resolveComponentFactory(FallingVaccineComponent);
    let componentFactory = this.vf.createComponent(resolver);
    setTimeout(()=>{
      componentFactory.destroy();
    }, 10000);
  }
}
