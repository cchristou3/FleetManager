import {Container} from "./container";

export interface OnShipUnloadedEvent {
  shipId: number;
  unloadedContainer: Container;
}

export function emptyShipUnloadedEvent(): OnShipUnloadedEvent{
  return { shipId: -1, unloadedContainer: { id: -1, name: '' } }
}
