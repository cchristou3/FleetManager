import {Component, OnDestroy, OnInit, ViewContainerRef} from '@angular/core';
import {NzMessageService} from "ng-zorro-antd/message";
import {ApiService} from "../../_services/api.service";
import {Subscription} from "rxjs";
import {Ship} from "../../_models/ship";
import {CreateShipFormComponent} from "../../_forms/create-form/create-ship-form.component";
import {LoadShipFormComponent} from "../../_forms/load-form/load-ship-form.component";
import {UnloadShipFormComponent} from "../../_forms/unload-form/unload-ship-form.component";
import {Container} from "../../_models/container";
import {TransferShipContainerFormComponent} from "../../_forms/transfer-form/transfer-ship-container-form.component";
import {ShipService} from "../../_services/hubs/ship.service";
import {OnShipLoadedEvent} from "../../_models/onShipLoadedEvent";
import {OnShipUnloadedEvent} from "../../_models/onShipUnloadedEvent";
import {onContainerTransferredEvent} from "../../_models/onContainerTransferredEvent";
import {ModalBuilderService} from "../../_services/modal-builder.service";

@Component({
  selector: 'app-ships',
  templateUrl: './ships.component.html',
  styleUrls: ['./ships.component.css'],
  providers: [ModalBuilderService]
})
export class ShipsComponent implements OnInit, OnDestroy {
  ships: Ship[] = [];

  isLoading = false
  subscriptions: Subscription[] = [];

  constructor(private message: NzMessageService,
              private apiService: ApiService,
              private shipService: ShipService,
              private modalBuilder: ModalBuilderService,
              viewContainerRef: ViewContainerRef) {
    modalBuilder.init(viewContainerRef)
    this.onShipLoaded = this.onShipLoaded.bind(this);
    this.onShipUnloaded = this.onShipUnloaded.bind(this);
    this.onContainerTransferred = this.onContainerTransferred.bind(this);
  }

  async ngOnInit() {
    this.isLoading = true;
    this.ships = await this.apiService.getShips(1, 1000);
    this.isLoading = false;
    this.shipService.createHubConnection()
    this.subscriptions.push(this.shipService.onShipLoaded$.subscribe(this.onShipLoaded));
    this.subscriptions.push(this.shipService.onShipUnloaded$.subscribe(this.onShipUnloaded));
    this.subscriptions.push(this.shipService.onContainerTransferred$.subscribe(this.onContainerTransferred));
  }

  ngOnDestroy() {
    this.shipService.stopHubConnection()
    for (const subscription of this.subscriptions)
      subscription.unsubscribe()
  }

  showCreateModal(): void {

    const onSave = (componentInstance: CreateShipFormComponent) => {
      if (!componentInstance.isValid())
        return;

      const request = componentInstance.prepareForApi();
      return this.apiService
        .addShip(request)
        .then((shipId) => {
          this.message.create('success', `The ship has been created!`);
          this.addToList(+shipId, request.name, request.capacity);
        })
    }

    this.modalBuilder.showModal('Create Ship', CreateShipFormComponent, onSave)
  }

  showLoadModal(shipId: number): void {
    const selectedShipName = this.getSelectedShipName(shipId);

    const onSave = (componentInstance: LoadShipFormComponent) => {
      if (!componentInstance.isValid())
        return;

      const request = componentInstance.prepareForApi();
      return this.apiService
        .loadShip(shipId, request, this.shipService.hubConnectionId)
        .then(_ => {
          const loadedShip = this.getShip(shipId)!;
          const selectedContainer = componentInstance.getSelectedContainer();

          this.message.create('success',
            `The container [${selectedContainer.name}] has been loaded to ship [${loadedShip.name}]!`);

          this.onContainerLoaded(loadedShip, selectedContainer);
        })
    }

    this.modalBuilder.showModal(`Loading Ship [${selectedShipName}]`, LoadShipFormComponent, onSave, null, '500px')
  }

  showUnloadModal(shipId: number): void {
    const selectedShip = this.ships.find(x => x.id === shipId)!;

    const onSave = (componentInstance: UnloadShipFormComponent) => {
      if (!componentInstance.isValid())
        return;

      const request = componentInstance.prepareForApi();
      return this.apiService
        .unloadShip(shipId, request, this.shipService.hubConnectionId)
        .then(_ => {
          const selectedContainer = componentInstance.getSelectedContainer();
          const loadedShip = this.getShip(shipId)!;
          this.message.create('success',
            `The container [${selectedContainer.name}] has been unloaded from ship [${loadedShip.name}]!`);
          this.onContainerUnloaded(loadedShip, selectedContainer);
        })
    }

    this.modalBuilder.showModal(`Unloading Ship [${selectedShip.name}]`, UnloadShipFormComponent, onSave, selectedShip.loadedContainers, '500px')
  }

  showTransferModal(sourceShipId: number): void {
    const selectedShipName = this.getSelectedShipName(sourceShipId);

    const onSave = (componentInstance: TransferShipContainerFormComponent) => {
      if (!componentInstance.isValid())
        return;

      const destinationShipId = componentInstance.getSelectedDestinationShipId();
      const request = componentInstance.prepareForApi();
      return this.apiService
        .transferShipContainer(sourceShipId, destinationShipId, request, this.shipService.hubConnectionId)
        .then(_ => {
          const sourceShip = this.getShip(sourceShipId)!;
          const destinationShip = this.getShip(destinationShipId)!;
          const selectedContainer = componentInstance.getSelectedContainer();

          this.onContainerUnloaded(sourceShip, selectedContainer);
          this.onContainerLoaded(destinationShip, selectedContainer);
        })
    }

    const data = {selectedShipId: sourceShipId, ships: this.ships}
    this.modalBuilder.showModal(`Transferring a Container of Ship [${selectedShipName}]`, TransferShipContainerFormComponent, onSave, data, '500px')
  }

  private getSelectedShipName(shipId: number) {
    return this.ships.find(x => x.id === shipId)!.name;
  }

  private addToList(id: number, name: string, capacity: number) {
    this.ships = [
      ...this.ships,
      new Ship(id, name, capacity, 0, []),
    ];
  }

  private onContainerLoaded(loadedShip: Ship, container: Container) {
    loadedShip.addContainer(container);
  }

  private getShip(shipId: number): Ship | undefined {
    return this.ships.find(x => x.id === shipId);
  }

  private onContainerUnloaded(loadedShip: Ship, container: Container) {
    loadedShip.removeContainer(container);
  }

  private onShipLoaded(data: OnShipLoadedEvent) {
    const loadedShip = this.getShip(data.shipId)!

    if (!loadedShip) return;

    this.message.create('success',
      `The container [${data.loadedContainer.name}] has been loaded to ship [${loadedShip.name}]!`);

    this.onContainerLoaded(loadedShip, data.loadedContainer);
  }

  private onShipUnloaded(data: OnShipUnloadedEvent) {
    const loadedShip = this.getShip(data.shipId)!

    if (!loadedShip) return;

    this.message.create('success',
      `The container [${data.unloadedContainer.name}] has been unloaded from ship [${loadedShip.name}]!`);

    this.onContainerUnloaded(loadedShip, data.unloadedContainer);
  }

  private onContainerTransferred(data: onContainerTransferredEvent) {
    const sourceShip = this.getShip(data.sourceShipId);
    const destinationShip = this.getShip(data.destinationShipId);

    if (sourceShip)
      this.onContainerUnloaded(sourceShip, data.selectedContainer);

    if (destinationShip)
      this.onContainerLoaded(destinationShip, data.selectedContainer);
  }
}
