import {Component, inject} from '@angular/core';
import {FormControl, FormGroup, NonNullableFormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {CreateShipRequest} from "../../_requests/CreateShipRequest";
import {NzInputModule} from "ng-zorro-antd/input";
import {NzFormModule} from "ng-zorro-antd/form";
import {IconsProviderModule} from "../../icons-provider.module";

@Component({
  selector: 'app-create-truck-form',
  styleUrls: [],
  standalone: true,
  imports: [
    NzInputModule,
    NzFormModule,
    ReactiveFormsModule,
    IconsProviderModule
  ],
  template: `
    <form nz-form [nzLayout]="'vertical'" [formGroup]="createForm">
      <nz-form-item>
        <nz-form-control nzErrorTip="Please input a truck name!">
          <nz-form-label nzNoColon="true" nzRequired nzFor="name">Name</nz-form-label>
          <nz-input-group>
            <input formControlName="name" nz-input/>
          </nz-input-group>
        </nz-form-control>
      </nz-form-item>
      <nz-form-item>
        <nz-form-control
          nzErrorTip="Please input a capacity!">
          <nz-form-label nzNoColon="true" nzRequired nzFor="capacity">Capacity</nz-form-label>
          <nz-input-group>
            <input type="number" min="1" max="3" formControlName="capacity" nz-input/>
          </nz-input-group>
        </nz-form-control>
      </nz-form-item>
    </form>
  `
})
export class CreateTruckFormComponent {

  constructor(private fb: NonNullableFormBuilder) {
  }

  createForm: FormGroup<{
    name: FormControl<string>;
    capacity: FormControl<number>;
  }> = this.fb.group({
    name: ['', [Validators.required]],
    capacity: [1, [Validators.required]],
  });

  validateForm() {
    if (this.createForm.valid)
      return true;

    Object
      .values(this.createForm.controls)
      .forEach(control => {
      if (control.invalid) {
        control.markAsDirty();
        control.updateValueAndValidity({onlySelf: true});
      }
    });

    return false;
  }

  prepareForApi(): CreateShipRequest {
    return {
      name: this.createForm.controls.name.value,
      capacity: this.createForm.controls.capacity .value,
    }
  }
}
