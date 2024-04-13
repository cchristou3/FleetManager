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

@Component({
  selector: 'app-load-ship-form',
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
    <form nz-form [nzLayout]="'vertical'" [formGroup]="loadForm">
      <nz-form-item>

        <nz-form-control nzErrorTip="Please choose a container!">
          <nz-form-label nzNoColon="true" nzRequired nzFor="containerId">Choose a container to load</nz-form-label>
          <nz-input-group>
            <nz-select formControlName="containerId" [nzLoading]="isLoading"
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
export class LoadShipFormComponent {

  isLoading = false;

  constructor(private fb: NonNullableFormBuilder, private apiService: ApiService) {
  }

  loadForm: FormGroup<{
    containerId: FormControl<string>;
  }> = this.fb.group({
    containerId: ['', [Validators.required]],
  });

  listOfOption: Container[] = [];

  async ngOnInit() {
    this.isLoading = true;
    this.listOfOption = await this.apiService.getContainers(1, 1000);
    this.isLoading = false;
  }

  validateForm() {
    if (this.loadForm.valid)
      return true;

    Object
      .values(this.loadForm.controls)
      .forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({onlySelf: true});
        }
      });

    return false;
  }

  prepareForApi() : LoadShipRequest {
    return { containerId: +this.loadForm.controls.containerId.value }
  }

  getSelectedContainer() : Container {
    return this.listOfOption.find(x => x.id == +this.loadForm.controls.containerId.value)!;
  }
}
