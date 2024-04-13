import {Component, inject} from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {NzInputModule} from "ng-zorro-antd/input";
import {NzFormModule} from "ng-zorro-antd/form";
import {NzSelectModule} from "ng-zorro-antd/select";
import {NgForOf} from "@angular/common";
import {ApiService} from "../../_services/api.service";
import {Ship} from "../../_models/ship";
import {Container} from "../../_models/container";
import {LoadShipRequest} from "../../_requests/LoadShipRequest";
import {NZ_MODAL_DATA} from "ng-zorro-antd/modal";
import {UnloadShipRequest} from "../../_requests/UnloadShipRequest";

@Component({
  selector: 'app-unload-ship-form',
  styleUrls: [],
  standalone: true,
  imports: [
    NzInputModule,
    NzFormModule,
    ReactiveFormsModule,
    NzSelectModule,
    FormsModule,
    NgForOf
  ],
  template: `
    <form nz-form [nzLayout]="'vertical'" [formGroup]="unloadForm">
      <nz-form-item>

        <nz-form-control nzErrorTip="Please choose a container!">
          <nz-form-label nzNoColon="true" nzRequired nzFor="containerId">Choose a container to unload</nz-form-label>
          <nz-input-group>
            <nz-select formControlName="containerId"
                       nzMode="default"
                       nzPlaceHolder="Please select">
              <nz-option *ngFor="let item of listOfOption"
                         [nzLabel]="item.name"
                         [nzValue]="item.id">
              </nz-option>
            </nz-select>
          </nz-input-group>
        </nz-form-control>

      </nz-form-item>
    </form>
  `
})
export class UnloadShipFormComponent {

  readonly inputData: Container[] = inject(NZ_MODAL_DATA);

  constructor(private fb: NonNullableFormBuilder, private apiService: ApiService) {
  }

  unloadForm: FormGroup<{
    containerId: FormControl<string>;
  }> = this.fb.group({
    containerId: ['', [Validators.required]],
  });

  listOfOption: Container[] = [];

  async ngOnInit() {
    this.listOfOption = this.inputData;
  }

  validateForm() {
    if (this.unloadForm.valid)
      return true;

    Object
      .values(this.unloadForm.controls)
      .forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({onlySelf: true});
        }
      });

    return false;
  }

  prepareForApi() : UnloadShipRequest {
    return { containerId: +this.unloadForm.controls.containerId.value }
  }

  getSelectedContainer() : Container {
    return this.listOfOption.find(x => x.id == +this.unloadForm.controls.containerId.value)!;
  }
}
