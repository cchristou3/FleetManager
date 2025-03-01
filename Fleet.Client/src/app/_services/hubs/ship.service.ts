import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import {HubService} from "./hub.service";
import {emptyShipLoadedEvent, OnShipLoadedEvent} from "../../_models/onShipLoadedEvent";
import {emptyShipUnloadedEvent, OnShipUnloadedEvent} from "../../_models/onShipUnloadedEvent";
import {emptyContainerTransferredEvent, onContainerTransferredEvent} from "../../_models/onContainerTransferredEvent";

@Injectable({
  providedIn: 'root'
})
export class ShipService extends HubService {
  private onShipLoadedSource = new BehaviorSubject<OnShipLoadedEvent>(emptyShipLoadedEvent());
  onShipLoaded$ = this.onShipLoadedSource.asObservable();

  private onShipUnloadedSource = new BehaviorSubject<OnShipUnloadedEvent>(emptyShipUnloadedEvent());
  onShipUnloaded$ = this.onShipUnloadedSource.asObservable();

  private onContainerTransferredSource = new BehaviorSubject<onContainerTransferredEvent>(emptyContainerTransferredEvent());
  onContainerTransferred$ = this.onContainerTransferredSource.asObservable();

  override createHubConnection() {
    console.log('createHubConnection')
    super.createHubConnection(`ships`);

    this.hubConnection.on('ShipLoaded', data => {
      this.onShipLoadedSource.next(data);
    });

    this.hubConnection.on('ShipUnloaded', data => {
      this.onShipUnloadedSource.next(data);
    });

    this.hubConnection.on('ContainerTransferred', data => {
      this.onContainerTransferredSource.next(data);
    });
  }
}
