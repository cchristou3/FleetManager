import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import {HubService} from "./hub.service";
import {Ship} from "../../_models/ship";
import {emptyShipLoadedEvent, OnShipLoadedEvent} from "../../_models/onShipLoadedEvent";
import {emptyShipUnloadedEvent, OnShipUnloadedEvent} from "../../_models/onShipUnloadedEvent";

@Injectable({
  providedIn: 'root'
})
export class ShipService extends HubService {
  private onShipLoadedSource = new BehaviorSubject<OnShipLoadedEvent>(emptyShipLoadedEvent());
  onShipLoaded$ = this.onShipLoadedSource.asObservable();

  private onShipUnloadedSource = new BehaviorSubject<OnShipUnloadedEvent>(emptyShipUnloadedEvent());
  onShipUnloaded$ = this.onShipUnloadedSource.asObservable();

  override createHubConnection() {
    console.log('createHubConnection')
    super.createHubConnection(`ships`);

    this.hubConnection.on('ShipLoaded', data => {
      this.onShipLoadedSource.next(data);
    });

    this.hubConnection.on('ShipUnloaded', data => {
      this.onShipUnloadedSource.next(data);
    });
  }
}
