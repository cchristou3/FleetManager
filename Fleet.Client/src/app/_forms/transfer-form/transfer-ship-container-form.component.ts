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
import {TransferShipContainerRequest} from "../../_requests/TransferShipContainerRequest";

@Component({
  selector: 'app-transfer-ship-container-form',
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
        <nz-form-control nzErrorTip="Please choose a container!">
          <nz-form-label nzNoColon="true" nzRequired nzFor="containerId">Choose a container to transfer</nz-form-label>
          <nz-input-group>
            <nz-select formControlName="containerId"
                       nzMode="default"
                       nzPlaceHolder="Please select">
              <nz-option *ngFor="let item of selectedShipContainers"
                         [nzLabel]="item.name"
                         [nzValue]="item.id">
              </nz-option>
            </nz-select>
          </nz-input-group>
        </nz-form-control>
      </nz-form-item>
      <nz-form-item>
        <nz-form-control nzErrorTip="Please choose a ship to transfer to!">
          <nz-form-label nzNoColon="true" nzRequired nzFor="destinationShipId">Choose a ship to transfer to</nz-form-label>
          <nz-input-group>
            <nz-select formControlName="destinationShipId"
                       nzMode="default"
                       nzPlaceHolder="Please select">
              <nz-option *ngFor="let item of destinationShips"
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
export class TransferShipContainerFormComponent {

  readonly inputData: { selectedShipId: number, ships: Ship[] } = inject(NZ_MODAL_DATA);

  constructor(private fb: NonNullableFormBuilder, private apiService: ApiService) {
  }

  transferForm: FormGroup<{
    containerId: FormControl<string>;
    destinationShipId: FormControl<string>;
  }> = this.fb.group({
    containerId: ['', [Validators.required]],
    destinationShipId: ['', [Validators.required]],
  });

  selectedShipContainers: Container[] = [];

  destinationShips: Ship[] = [];

  async ngOnInit() {
    const ships = this.inputData.ships.filter(x => x.id !== this.inputData.selectedShipId)

    const selectedShipContainers = this.inputData.ships
      .filter(x => x.id === this.inputData.selectedShipId)[0].loadedContainers

    this.destinationShips = ships;
    this.selectedShipContainers = selectedShipContainers;
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

  prepareForApi() : TransferShipContainerRequest {
    return { containerId: +this.transferForm.controls.containerId.value }
  }

  getSelectedContainer() : Container {
    return this.selectedShipContainers.find(x => x.id == +this.transferForm.controls.containerId.value)!;
  }

  getSelectedDestinationShipId() : number {
    return +this.transferForm.controls.destinationShipId.value;
  }
}
