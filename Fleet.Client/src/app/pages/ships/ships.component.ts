import {Component, OnDestroy, OnInit, ViewContainerRef} from '@angular/core';
import {NzMessageService} from "ng-zorro-antd/message";
import {NzModalService} from "ng-zorro-antd/modal";
import {ApiService} from "../../_services/api.service";
import {Subscription} from "rxjs";
import {Ship} from "../../_models/ship";
import {CreateShipFormComponent} from "../../_forms/create-form/create-ship-form.component";
import {NzSafeAny} from "ng-zorro-antd/core/types";
import {LoadShipFormComponent} from "../../_forms/load-form/load-ship-form.component";
import {UnloadShipFormComponent} from "../../_forms/unload-form/unload-ship-form.component";
import {Container} from "../../_models/container";
import {TransferShipContainerFormComponent} from "../../_forms/transfer-form/transfer-ship-container-form.component";
import {ShipService} from "../../_services/hubs/ship.service";
import {OnShipLoadedEvent} from "../../_models/onShipLoadedEvent";
import {OnShipUnloadedEvent} from "../../_models/onShipUnloadedEvent";
import {onContainerTransferredEvent} from "../../_models/onContainerTransferredEvent";

@Component({
  selector: 'app-ships',
  templateUrl: './ships.component.html',
  styleUrls: ['./ships.component.css']
})
export class ShipsComponent implements OnInit, OnDestroy {
  ships: Ship[] = [];

  isLoading = false
  subscriptions: Subscription[] = [];

  constructor(private message: NzMessageService,
              private modal: NzModalService,
              private viewContainerRef: ViewContainerRef,
              private apiService: ApiService,
              private shipService: ShipService) {
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

  ngOnDestroy(){
    this.shipService.stopHubConnection()
    for (const subscription of this.subscriptions)
      subscription.unsubscribe()
  }

  showCreateModal(): void {
    const modal = this.modal.create<CreateShipFormComponent>({
      nzTitle: 'Create Ship',
      nzWidth: '350px',
      nzContent: CreateShipFormComponent,
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
          onClick: async (componentInstance: CreateShipFormComponent) =>  {
            const isValid = componentInstance.validateForm();
            if (!isValid)
              return;

            const request = componentInstance.prepareForApi();
            await this.apiService
              .addShip(request)
              .then((shipId) => {
                this.message.create('success', `The ship has been created!`);
                this.addToList(+shipId, request.name, request.capacity);
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

  showLoadModal(shipId: number): void {
    const selectedShipName = this.getSelectedShipName(shipId);
    const modal = this.modal.create<LoadShipFormComponent>({
      nzTitle: `Loading Ship [${selectedShipName}]`,
      nzWidth: '500px',
      nzContent: LoadShipFormComponent,
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
          onClick: async (componentInstance: LoadShipFormComponent) =>  {
            const isValid = componentInstance.validateForm();
            if (!isValid)
              return;

            const request = componentInstance.prepareForApi();
            await this.apiService
              .loadShip(shipId, request, this.shipService.hubConnectionId)
              .then(_ => {
                const loadedShip = this.getShip(shipId)!;
                const selectedContainer = componentInstance.getSelectedContainer();

                this.message.create('success',
                  `The container [${selectedContainer.name}] has been loaded to ship [${loadedShip.name}]!`);

                this.onContainerLoaded(loadedShip, selectedContainer);
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

  showUnloadModal(shipId: number): void {
    const selectedShip = this.ships.find(x => x.id === shipId)!;
    const modal = this.modal.create<UnloadShipFormComponent>({
      nzTitle: `Unloading Ship [${selectedShip.name}]`,
      nzWidth: '500px',
      nzContent: UnloadShipFormComponent,
      nzViewContainerRef: this.viewContainerRef,
      nzData: selectedShip.loadedContainers,
      nzFooter: [
        {
          label: 'Cancel',
          onClick(): NzSafeAny | Promise<NzSafeAny> {
            modal.destroy();
          }
        },
        {
          label: 'Save',
          onClick: async (componentInstance: UnloadShipFormComponent) =>  {
            const isValid = componentInstance.validateForm();
            if (!isValid)
              return;

            const request = componentInstance.prepareForApi();
            await this.apiService
              .unloadShip(shipId, request, this.shipService.hubConnectionId)
              .then(_ => {
                const selectedContainer = componentInstance.getSelectedContainer();
                const loadedShip = this.getShip(shipId)!;
                this.message.create('success',
                  `The container [${selectedContainer.name}] has been unloaded from ship [${loadedShip.name}]!`);
                this.onContainerUnloaded(loadedShip, selectedContainer);
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

  showTransferModal(sourceShipId: number): void {
    const selectedShipName = this.getSelectedShipName(sourceShipId);
    const modal = this.modal.create<TransferShipContainerFormComponent>({
      nzTitle: `Transferring a Container of Ship [${selectedShipName}]`,
      nzWidth: '500px',
      nzContent: TransferShipContainerFormComponent,
      nzViewContainerRef: this.viewContainerRef,
      nzData: {
        selectedShipId: sourceShipId,
        ships: this.ships
      },
      nzFooter: [
        {
          label: 'Cancel',
          onClick(): NzSafeAny | Promise<NzSafeAny> {
            modal.destroy();
          }
        },
        {
          label: 'Save',
          onClick: async (componentInstance: TransferShipContainerFormComponent) =>  {
            const isValid = componentInstance.validateForm();
            if (!isValid)
              return;

            const destinationShipId = componentInstance.getSelectedDestinationShipId();
            const request = componentInstance.prepareForApi();
            await this.apiService
              .transferShipContainer(sourceShipId, destinationShipId, request, this.shipService.hubConnectionId)
              .then(_ => {
                const sourceShip = this.getShip(sourceShipId)!;
                const destinationShip = this.getShip(destinationShipId)!;
                const selectedContainer = componentInstance.getSelectedContainer();

                this.onContainerUnloaded(sourceShip, selectedContainer);
                this.onContainerLoaded(destinationShip, selectedContainer);

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

  private getSelectedShipName(shipId: number) {
    return this.ships.find(x => x.id === shipId)!.name;
  }


  private addToList(id: number, name: string, capacity: number) {
    this.ships = [
      ... this.ships,
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

  private async onShipLoaded(data: OnShipLoadedEvent) {
    const loadedShip = this.getShip(data.shipId)!

    if (!loadedShip) return;

    this.message.create('success',
      `The container [${data.loadedContainer.name}] has been loaded to ship [${loadedShip.name}]!`);

    this.onContainerLoaded(loadedShip, data.loadedContainer);
  }

  private async onShipUnloaded(data: OnShipUnloadedEvent) {
    const loadedShip = this.getShip(data.shipId)!

    if (!loadedShip) return;

    this.message.create('success',
      `The container [${data.unloadedContainer.name}] has been unloaded from ship [${loadedShip.name}]!`);

    this.onContainerUnloaded(loadedShip, data.unloadedContainer);
  }

  private async onContainerTransferred(data: onContainerTransferredEvent) {
    const sourceShip = this.getShip(data.sourceShipId);
    const destinationShip = this.getShip(data.destinationShipId);

    if (sourceShip)
      this.onContainerUnloaded(sourceShip, data.selectedContainer);

    if (destinationShip)
      this.onContainerLoaded(destinationShip, data.selectedContainer);
  }
}
