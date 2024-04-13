import { ContainersRoutingModule } from './containers-routing.module';
import { ContainersComponent } from './containers.component';
import {SharedModule} from "../../shared/shared.module";
import {NgModule} from "@angular/core";
import {NgForOf} from "@angular/common";


@NgModule({
  imports: [ContainersRoutingModule, SharedModule, NgForOf],
  declarations: [ContainersComponent],
  exports: [ContainersComponent]
})
export class ContainersModule { }
