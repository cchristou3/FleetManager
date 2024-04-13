import { NgModule } from '@angular/core';

import { ShipsRoutingModule } from './ships-routing.module';

import { ShipsComponent } from './ships.component';
import {SharedModule} from "../../shared/shared.module";
import {NgForOf} from "@angular/common";


@NgModule({
  imports: [ShipsRoutingModule, SharedModule, NgForOf],
  declarations: [ShipsComponent],
  exports: [ShipsComponent]
})
export class ShipsModule { }
