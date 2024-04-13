import {Component, OnInit, ViewContainerRef} from '@angular/core';
import {Truck} from "../../_models/truck";
import {NzMessageService} from "ng-zorro-antd/message";
import {NzModalService} from "ng-zorro-antd/modal";
import {ApiService} from "../../_services/api.service";
import {firstValueFrom} from "rxjs";
import {CreateTruckFormComponent} from "../../_forms/create-form/create-truck-form.component";
import {NzSafeAny} from "ng-zorro-antd/core/types";
import {LoadTruckFormComponent} from "../../_forms/load-form/load-truck-form.component";
import {TruckContainer} from "../../_models/truckContainer";
import {Container} from "../../_models/container";
import {UnloadTruckRequest} from "../../_requests/UnloadTruckRequest";
import {TransferTruckContainerFormComponent} from "../../_forms/transfer-form/transfer-truck-container-form.component";

@Component({
  selector: 'app-trucks',
  templateUrl: './trucks.component.html',
  styleUrls: ['./trucks.component.css']
})
export class TrucksComponent implements OnInit {
  trucks: Truck[] = [];

  isLoading = false

  constructor(private message: NzMessageService,
              private modal: NzModalService,
              private viewContainerRef: ViewContainerRef,
              private apiService: ApiService) {
  }

  async ngOnInit() {
    this.isLoading = true;
    this.trucks = await this.apiService.getTrucks(1, 10);
    this.isLoading = false;
  }

  showCreateModal(): void {
    const modal = this.modal.create<CreateTruckFormComponent>({
      nzTitle: 'Create Truck',
      nzWidth: '350px',
      nzContent: CreateTruckFormComponent,
      nzViewContainerRef: this.viewContainerRef,
      nzFooter: [
        {
          label: 'Cancel',
          onClick(): NzSafeAny | Promise<NzSafeAny> {
            modal.destroy();
          }
        },
        {
          label: 'Save',
          onClick: async (componentInstance: CreateTruckFormComponent) =>  {
            const isValid = componentInstance.validateForm();
            if (!isValid)
              return;

            const request = componentInstance.prepareForApi();
            await this.apiService
              .addTruck(request)
              .then((truckId) => {
                this.message.create('success', `The truck has been created!`);
                this.addToList(+truckId, request.name, request.capacity);
                modal.destroy();
              })
              .catch(e => {
                this.message.error(e.error.message)
              })
          }
        }
      ]
    });
  }

  showLoadModal(truckId: number): void {
    const selectedTruckName = this.getSelectedTruckName(truckId);
    const modal = this.modal.create<LoadTruckFormComponent>({
      nzTitle: `Loading Truck [${selectedTruckName}]`,
      nzWidth: '500px',
      nzContent: LoadTruckFormComponent,
      nzViewContainerRef: this.viewContainerRef,
      nzFooter: [
        {
          label: 'Cancel',
          onClick(): NzSafeAny | Promise<NzSafeAny> {
            modal.destroy();
          }
        },
        {
          label: 'Save',
          onClick: async (componentInstance: LoadTruckFormComponent) =>  {
            const isValid = componentInstance.validateForm();
            if (!isValid)
              return;

            const request = componentInstance.prepareForApi();
            await this.apiService
              .loadTruck(truckId, request)
              .then(_ => {
                const loadedTruck = this.getTruck(truckId);
                const selectedContainer = componentInstance.getSelectedContainer();

                this.message.create('success',
                  `The container [${selectedContainer.name}] has been loaded to truck [${loadedTruck.name}]!`);

                this.onContainerLoaded(loadedTruck, selectedContainer);
                modal.destroy();
              })
              .catch(e => {
                this.message.error(e.error.message)
              })
          }
        }
      ]
    });
  }

  showUnloadModal(truckId: number): void {
    const latestLoadedContainer = this.trucks.find(x => x.id === truckId)!.getLatestLoadedContainer();
    const confirmModal = this.modal.confirm({
      nzTitle: 'Do you want to unload the latest container?',
      nzContent: `[${latestLoadedContainer.name}] is the latest loaded container. When "Yes, unload" gets clicked then it will be unloaded.`,
      nzOkText: 'Yes, unload',
      nzOnOk: _ => {
        const request:UnloadTruckRequest = {
          containerId: latestLoadedContainer.containerId
        };
        return this.apiService
          .unloadTruck(truckId, request)
          .then(d => {
            const loadedTruck = this.getTruck(truckId);
            this.message.create('success',
              `The container [${latestLoadedContainer.name}] has been unloaded from truck [${loadedTruck.name}]!`);

            this.onContainerUnloaded(loadedTruck, latestLoadedContainer);
          })
        .catch(e => this.message.error(e.error.message));
      }
    });
  }

  showTransferModal(sourceTruckId: number): void {
    const selectedTruckName = this.getSelectedTruckName(sourceTruckId);
    const modal = this.modal.create<TransferTruckContainerFormComponent>({
      nzTitle: `Transferring Latest Container of Truck [${selectedTruckName}]`,
      nzWidth: '500px',
      nzContent: TransferTruckContainerFormComponent,
      nzViewContainerRef: this.viewContainerRef,
      nzData: { selectedTruckId: sourceTruckId, trucks: this.trucks },
      nzFooter: [
        {
          label: 'Cancel',
          onClick(): NzSafeAny | Promise<NzSafeAny> {
            modal.destroy();
          }
        },
        {
          label: 'Save',
          onClick: async (componentInstance: TransferTruckContainerFormComponent) =>  {
            const isValid = componentInstance.validateForm();
            if (!isValid)
              return;

            const destinationTruckId = componentInstance.getSelectedDestinationTruckId()
            const request = componentInstance.prepareForApi();
            await this.apiService
              .transferTruckContainer(sourceTruckId, destinationTruckId, request)
              .then(_ => {
                const sourceTruck = this.getTruck(sourceTruckId);
                const destinationTruck = this.getTruck(destinationTruckId);
                const selectedTruckContainer = componentInstance.getSelectedContainer();

                this.message.create('success',
                  `The container [${selectedTruckContainer.name}] has been loaded to truck [${sourceTruck.name}]!`);

                this.onContainerUnloaded(sourceTruck, selectedTruckContainer);
                this.onContainerLoaded(destinationTruck, this.toContainer(selectedTruckContainer));
                modal.destroy();
              })
              .catch(e => {
                this.message.error(e.error.message)
              })
          }
        }
      ]
    });
  }

  private toContainer(truckContainer: TruckContainer): Container {
    return {
      id: truckContainer.containerId,
      name: truckContainer.name
    }
  }

  private getSelectedTruckName(truckId: number) {
    return this.trucks.find(x => x.id === truckId)!.name;
  }


  private addToList(id: number, name: string, capacity: number) {
    this.trucks = [
      ... this.trucks,
      new Truck(id, name, capacity, 0, []),
    ];
  }

  private onContainerLoaded(loadedTruck: Truck, container: Container) {
    loadedTruck.addContainer(container);
  }

  private getTruck(truckId: number) {
    return this.trucks.find(x => x.id === truckId)!;
  }

  private onContainerUnloaded(loadedTruck: Truck, container: TruckContainer) {
    loadedTruck.removeContainer(container);
  }
}
