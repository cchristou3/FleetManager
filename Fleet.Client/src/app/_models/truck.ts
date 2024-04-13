import {Container} from "./container";
import {TruckContainer} from "./truckContainer";



export class Truck {

  public id: number;
  public name: string;
  public maximumCapacity: number;
  public currentCapacity: number;
  public loadedContainers: TruckContainer[];
  public loadedContainersCommaSeparated: string = ''

  constructor(
    id: number,
    name: string,
    maximumCapacity: number,
    currentCapacity: number,
    loadedContainers: TruckContainer[]
  ) {
    this.id = id;
    this.name = name;
    this.maximumCapacity = maximumCapacity;
    this.currentCapacity = currentCapacity;
    this.loadedContainers = loadedContainers;
    this.loadedContainersCommaSeparated = '';
    this.updateLoadedContainersCommaSeparated();
  }

  public addContainer(container: Container) {
    this.currentCapacity++;

    const truckContainer: TruckContainer = {
      containerId: container.id,
      name: container.name,
      dateLoaded: this.generateDateLoaded()
    };

    this.loadedContainers.push(truckContainer)
    this.updateLoadedContainersCommaSeparated();
  }

  public removeContainer(container: TruckContainer) {
    this.currentCapacity--;
    this.loadedContainers = this.loadedContainers.filter(x => x.containerId !== container.containerId)
    this.updateLoadedContainersCommaSeparated();
  }

  public updateLoadedContainersCommaSeparated() {
    this.loadedContainersCommaSeparated = this.loadedContainers.map(x => x.name).join(', ');
  }

  public getLatestLoadedContainer(): TruckContainer {
    const loadedContainers = [...this.loadedContainers];
    loadedContainers.sort((a, b) => {
      return new Date(b.dateLoaded).getTime() - new Date(a.dateLoaded).getTime()
    });
    return loadedContainers[0];
  }

  private generateDateLoaded() {
    const latestContainer = this.getLatestLoadedContainer();
    return latestContainer ?
      new Date(latestContainer.dateLoaded.getTime() + 1) :
      new Date()
  }
}
