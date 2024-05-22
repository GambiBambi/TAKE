import { Component } from '@angular/core';
import {formatDate} from "@angular/common";

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html'
})
export class AboutComponent {
  todayDate = new Date(Date.now()).toString();
}