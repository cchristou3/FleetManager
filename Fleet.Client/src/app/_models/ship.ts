import {Container} from "./container";

export class Ship {
  public id: number;
  public name: string;
  public maximumCapacity: number;
  public currentCapacity: number;
  public loadedContainers: Container[];
  public loadedContainersCommaSeparated: string = ''

  constructor(
    id: number,
    name: string,
    maximumCapacity: number,
    currentCapacity: number,
    loadedContainers: Container[]
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
    this.loadedContainers.push(container)
    this.updateLoadedContainersCommaSeparated();
  }

  public removeContainer(container: Container) {
    this.currentCapacity--;
    this.loadedContainers = this.loadedContainers.filter(x => x.id !== container.id)
    this.updateLoadedContainersCommaSeparated();
  }

  public updateLoadedContainersCommaSeparated() {
    this.loadedContainersCommaSeparated = this.loadedContainers.map(x => x.name).join(', ');
  }
}
