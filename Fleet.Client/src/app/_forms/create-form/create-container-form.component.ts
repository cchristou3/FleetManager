import {Component} from '@angular/core';
import {FormControl, FormGroup, NonNullableFormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {CreateContainerRequest} from "../../_requests/CreateContainerRequest";
import {NzFormModule} from "ng-zorro-antd/form";
import {NzInputModule} from "ng-zorro-antd/input";
import {IconsProviderModule} from "../../icons-provider.module";
@Component({
  selector: 'app-create-container-form',
  styleUrls: [],
  standalone: true,
  imports: [
    NzFormModule,
    NzInputModule,
    ReactiveFormsModule,
    IconsProviderModule
  ],
  template: `
    <form nz-form [nzLayout]="'inline'" [formGroup]="createForm">
      <nz-form-item>
        <nz-form-control nzErrorTip="Please input a container name!">
          <nz-form-label nzNoColon="true" nzRequired nzFor="name">Name</nz-form-label>
          <nz-input-group>
            <input formControlName="name" nz-input/>
          </nz-input-group>
        </nz-form-control>
      </nz-form-item>
    </form>
  `
})
export class CreateContainerFormComponent {

  constructor(private fb: NonNullableFormBuilder) {
  }

  createForm: FormGroup<{
    name: FormControl<string>;
  }> = this.fb.group({
    name: ['', [Validators.required]],
  });

  isValid() {
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

  prepareForApi(): CreateContainerRequest {
    return {name: this.createForm.controls.name.value}
  }
}
