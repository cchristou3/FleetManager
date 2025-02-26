import {Container} from "./container";

export interface OnShipLoadedEvent {
  shipId: number;
  loadedContainer: Container;
}

export function emptyShipLoadedEvent(): OnShipLoadedEvent{
  return { shipId: -1, loadedContainer: { id: -1, name: '' } }
}
