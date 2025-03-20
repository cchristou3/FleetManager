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
import {Truck} from "../../_models/truck";
import {NZ_MODAL_DATA} from "ng-zorro-antd/modal";
import {TruckContainer} from "../../_models/truckContainer";
import {TransferTruckContainerRequest} from "../../_requests/TransferTruckContainerRequest";

@Component({
  selector: 'app-transfer-truck-container-form',
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
    <form nz-form [nzLayout]="'vertical'" [formGroup]="transferForm">
      <nz-form-item>
        <nz-form-control>
          <nz-form-label nzNoColon="true" nzRequired nzFor="containerName">Container to transfer</nz-form-label>
          <nz-input-group>
            <input readonly="true" formControlName="containerName" nz-input [value]="latestLoadedContainer?.name"/>
          </nz-input-group>
        </nz-form-control>
      </nz-form-item>
      <nz-form-item>
        <nz-form-control nzErrorTip="Please choose a truck to transfer to!">
          <nz-form-label nzNoColon="true" nzRequired nzFor="destinationTruckId">Choose a truck to transfer to
          </nz-form-label>
          <nz-input-group>
            <nz-select formControlName="destinationTruckId"
                       nzMode="default"
                       nzPlaceHolder="Please select">
              <nz-option *ngFor="let item of destinationTrucks"
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
export class TransferTruckContainerFormComponent {

  readonly inputData: { selectedTruckId: number, trucks: Truck[] } = inject(NZ_MODAL_DATA);

  constructor(private fb: NonNullableFormBuilder, private apiService: ApiService) {
  }

  transferForm: FormGroup<{
    containerName: FormControl<string>;
    destinationTruckId: FormControl<string>;
  }> = this.fb.group({
    containerName: ['', [Validators.required]],
    destinationTruckId: ['', [Validators.required]],
  });

  destinationTrucks: Truck[] = [];

  latestLoadedContainer?: TruckContainer;

  async ngOnInit() {

    this.destinationTrucks = this.inputData.trucks.filter(x => x.id !== this.inputData.selectedTruckId);

    this.latestLoadedContainer = this.inputData.trucks
      .filter(x => x.id === this.inputData.selectedTruckId)[0]
      .getLatestLoadedContainer();

    this.transferForm.controls.containerName.setValue(this.latestLoadedContainer.name);
  }

  isValid() {
    if (this.transferForm.valid)
      return true;

    Object
      .values(this.transferForm.controls)
      .forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({onlySelf: true});
        }
      });

    return false;
  }

  prepareForApi() : TransferTruckContainerRequest {
    return { containerId: +this.latestLoadedContainer!.containerId! }
  }

  getSelectedDestinationTruckId() : number {
    return +this.transferForm.controls.destinationTruckId.value;
  }

  getSelectedContainer() : TruckContainer {
    return this.latestLoadedContainer!
  }
}
