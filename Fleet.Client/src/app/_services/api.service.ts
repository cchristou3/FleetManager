import { Injectable } from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {catchError, firstValueFrom, map, Observable, of} from "rxjs";
import {Container} from "../_models/container";
import {CreateContainerRequest} from "../_requests/CreateContainerRequest";
import {CreateShipRequest} from "../_requests/CreateShipRequest";
import {Ship} from "../_models/ship";
import {Truck} from "../_models/truck";
import {CreateTruckRequest} from "../_requests/CreateTruckRequest";
import {LoadShipRequest} from "../_requests/LoadShipRequest";
import {UnloadShipRequest} from "../_requests/UnloadShipRequest";
import {LoadTruckRequest} from "../_requests/LoadTruckRequest";
import {UnloadTruckRequest} from "../_requests/UnloadTruckRequest";
import {TransferShipContainerRequest} from "../_requests/TransferShipContainerRequest";
import {TransferTruckContainerRequest} from "../_requests/TransferTruckContainerRequest";
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  apiUrl: string = environment.apiBaseUrl;

  constructor(private http: HttpClient) {
  }

  public getQueryParams(pageNumber: number, pageSize: number){
    return  new HttpParams()
      .append('page', `${pageNumber}`)
      .append('results', `${pageSize}`);
  }

  getContainers(
    pageNumber: number,
    pageSize: number
  ): Promise<Container[]> {
    const params = this.getQueryParams(pageNumber, pageSize);
    return firstValueFrom(this.http
      .get<Container[]>(`${this.apiUrl}/Containers`, { params }))
  }

  addContainer(
    request: CreateContainerRequest
  ) : Promise<Object> {
    return firstValueFrom(this.http
      .post(`${this.apiUrl}/Containers`, request))
  }

  getShips(
    pageNumber: number,
    pageSize: number
  ): Promise<Ship[]> {
    const params = this.getQueryParams(pageNumber, pageSize);
    return firstValueFrom(this.http
      .get<Ship[]>(`${this.apiUrl}/Ships`, { params })
      .pipe(map(x => {
        return x.map(el => {
          const ship = new Ship(el.id, el.name, el.maximumCapacity, el.currentCapacity, el.loadedContainers);
          ship.updateLoadedContainersCommaSeparated()
          return ship;
        })
      })))
  }

  addShip(
    request: CreateShipRequest
  ) : Promise<Object> {
    return firstValueFrom(this.http
      .post(`${this.apiUrl}/Ships`, request))
  }

  loadShip(
    shipId: number,
    request: LoadShipRequest
  ) : Promise<Object> {
    return firstValueFrom(this.http
      .post(`${this.apiUrl}/Ships/${shipId}/load`, request))
  }

  unloadShip(
    shipId: number,
    request: UnloadShipRequest
  ) : Promise<Object> {
    return firstValueFrom(this.http
      .post(`${this.apiUrl}/Ships/${shipId}/unload`, request))
  }

  transferShipContainer(
    sourceShipId: number,
    destinationShipId: number,
    request: TransferShipContainerRequest
  ) : Promise<Object> {
    return firstValueFrom(this.http
      .post(`${this.apiUrl}/Ships/${sourceShipId}/transfer/${destinationShipId}`, request))
  }

  getTrucks(
    pageNumber: number,
    pageSize: number
  ): Promise<Truck[]> {
    const params = this.getQueryParams(pageNumber, pageSize);
    return firstValueFrom(this.http
      .get<Truck[]>(`${this.apiUrl}/Trucks`, { params })
      .pipe(map(x => {
        return x.map(el => {
          const truck = new Truck(el.id, el.name, el.maximumCapacity, el.currentCapacity, el.loadedContainers);
          truck.updateLoadedContainersCommaSeparated()
          return truck;
        })
      })))
  }

  addTruck(
    request: CreateTruckRequest
  ) : Promise<Object> {
    return firstValueFrom(this.http
      .post(`${this.apiUrl}/Trucks`, request))
  }

  loadTruck(
    truckId: number,
    request: LoadTruckRequest
  ) : Promise<Object> {
    return firstValueFrom(this.http
      .post(`${this.apiUrl}/Trucks/${truckId}/load`, request))
  }

  unloadTruck(
    truckId: number,
    request: UnloadTruckRequest
  ) : Promise<Object> {
    return firstValueFrom(this.http
      .post(`${this.apiUrl}/Trucks/${truckId}/unload`, request))
  }

  transferTruckContainer(
    sourceTruckId: number,
    destinationTruckId: number,
    request: TransferTruckContainerRequest
  ) : Promise<Object> {
    return firstValueFrom(this.http
      .post(`${this.apiUrl}/Trucks/${sourceTruckId}/transfer/${destinationTruckId}`, request))
  }

}
