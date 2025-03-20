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
import {ModalBuilderService} from "../../_services/modal-builder.service";
import {CreateContainerFormComponent} from "../../_forms/create-form/create-container-form.component";

@Component({
  selector: 'app-trucks',
  templateUrl: './trucks.component.html',
  styleUrls: ['./trucks.component.css'],
  providers: [ModalBuilderService]
})
export class TrucksComponent implements OnInit {
  trucks: Truck[] = [];

  isLoading = false

  constructor(private message: NzMessageService,
              private apiService: ApiService,
              private modalBuilder: ModalBuilderService,
              viewContainerRef: ViewContainerRef) {
    modalBuilder.init(viewContainerRef)
  }

  async ngOnInit() {
    this.isLoading = true;
    this.trucks = await this.apiService.getTrucks(1, 10);
    this.isLoading = false;
  }

  showCreateModal(): void {

    const onSave = async (componentInstance: CreateTruckFormComponent) =>  {
      if (!componentInstance.isValid())
        return;

      const request = componentInstance.prepareForApi();
      return this.apiService
        .addTruck(request)
        .then((truckId) => {
          this.message.create('success', `The truck has been created!`);
          this.addToList(+truckId, request.name, request.capacity);
        })
    }

    this.modalBuilder.showModal('Create Truct', CreateTruckFormComponent, onSave)
  }

  showLoadModal(truckId: number): void {
    const selectedTruckName = this.getSelectedTruckName(truckId);

    const onSave = async (componentInstance: LoadTruckFormComponent) =>  {
      if (!componentInstance.isValid())
        return;

      const request = componentInstance.prepareForApi();
      return this.apiService
        .loadTruck(truckId, request)
        .then(_ => {
          const loadedTruck = this.getTruck(truckId);
          const selectedContainer = componentInstance.getSelectedContainer();

          this.message.create('success',
            `The container [${selectedContainer.name}] has been loaded to truck [${loadedTruck.name}]!`);

          this.onContainerLoaded(loadedTruck, selectedContainer);
        })
    }

    this.modalBuilder.showModal(`Loading Truck [${selectedTruckName}]`, LoadTruckFormComponent, onSave, null, '500px')
  }

  showUnloadModal(truckId: number): void {
    const latestLoadedContainer = this.trucks.find(x => x.id === truckId)!.getLatestLoadedContainer();

    const onOk = () => {
      const request: UnloadTruckRequest = {
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
    }

    this.modalBuilder.showConfirmationModal(
      'Do you want to unload the latest container?',
      `[${latestLoadedContainer.name}] is the latest loaded container. When "Yes, unload" gets clicked then it will be unloaded.`,
      'Yes, unload',
      onOk
    )
  }

  showTransferModal(sourceTruckId: number): void {
    const selectedTruckName = this.getSelectedTruckName(sourceTruckId);

    const onSave = async (componentInstance: TransferTruckContainerFormComponent) =>  {
      if (!componentInstance.isValid())
        return;

      const destinationTruckId = componentInstance.getSelectedDestinationTruckId()
      const request = componentInstance.prepareForApi();
      return this.apiService
        .transferTruckContainer(sourceTruckId, destinationTruckId, request)
        .then(_ => {
          const sourceTruck = this.getTruck(sourceTruckId);
          const destinationTruck = this.getTruck(destinationTruckId);
          const selectedTruckContainer = componentInstance.getSelectedContainer();

          this.message.create('success',
            `The container [${selectedTruckContainer.name}] has been loaded to truck [${sourceTruck.name}]!`);

          this.onContainerUnloaded(sourceTruck, selectedTruckContainer);
          this.onContainerLoaded(destinationTruck, this.toContainer(selectedTruckContainer));
        })
    }

    const data = { selectedTruckId: sourceTruckId, trucks: this.trucks }
    this.modalBuilder.showModal(`Transferring Latest Container of Truck [${selectedTruckName}]`, TransferTruckContainerFormComponent, onSave, data, '500px')
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
