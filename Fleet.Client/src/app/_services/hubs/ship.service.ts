import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import {HubService} from "./hub.service";
import {Ship} from "../../_models/ship";
import {emptyEvent, OnShipLoadedEvent} from "../../_models/onShipLoadedEvent";

@Injectable({
  providedIn: 'root'
})
export class ShipService extends HubService {
  private onShipLoadedSource = new BehaviorSubject<OnShipLoadedEvent>(emptyEvent());
  onShipLoaded$ = this.onShipLoadedSource.asObservable();

  override createHubConnection() {
    console.log('createHubConnection')
    super.createHubConnection(`ships`);

    this.hubConnection.on('ShipLoaded', data => {
      this.onShipLoadedSource.next(data);
    });
  }
}
