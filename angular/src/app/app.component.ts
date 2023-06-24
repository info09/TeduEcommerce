import { Component } from '@angular/core';
import { PrimeNGConfig } from 'primeng/api';

@Component({
  selector: 'app-root',
  template: `
    <router-outlet></router-outlet>
  `,
})
export class AppComponent {
  menuMode = 'static';

  constructor(private primengConfig: PrimeNGConfig){}

  ngOnInit(): void {
    //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
    //Add 'implements OnInit' to the class.
    this.primengConfig.ripple = true;
    document.documentElement.style.fontSize = '14px';
  }
}
