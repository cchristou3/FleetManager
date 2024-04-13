import { NgModule } from '@angular/core';

import { TrucksRoutingModule } from './trucks-routing.module';

import { TrucksComponent } from './trucks.component';
import {NzSpinModule} from "ng-zorro-antd/spin";
import {NgForOf} from "@angular/common";
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzIconModule} from "ng-zorro-antd/icon";
import {NzTableModule} from "ng-zorro-antd/table";
import {NzTransitionPatchModule} from "ng-zorro-antd/core/transition-patch/transition-patch.module";
import {NzWaveModule} from "ng-zorro-antd/core/wave";
import {SharedModule} from "../../shared/shared.module";


@NgModule({
  imports: [TrucksRoutingModule, SharedModule, NgForOf],
  declarations: [TrucksComponent],
  exports: [TrucksComponent]
})
export class TrucksModule { }
