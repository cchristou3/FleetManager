import {NgModule} from "@angular/core";
import {NgForOf, NgIf} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {NzInputModule} from "ng-zorro-antd/input";
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzIconModule} from "ng-zorro-antd/icon";
import {NzDividerModule} from "ng-zorro-antd/divider";
import {NzTableModule} from "ng-zorro-antd/table";
import {NzFormModule} from "ng-zorro-antd/form";
import {IconsProviderModule} from "../icons-provider.module";


const modules = [
  NzDividerModule,
  NzTableModule,
  FormsModule,
  NzInputModule,
  NzButtonModule,
  NzIconModule,
  NzFormModule,
  ReactiveFormsModule,
  IconsProviderModule
]

@NgModule({
  imports: modules,
  declarations: [],
  exports: modules
})
export class SharedModule { }
