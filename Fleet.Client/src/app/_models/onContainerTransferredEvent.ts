import {Container} from "./container";

export interface onContainerTransferredEvent {
  sourceShipId: number;
  selectedContainer: Container;
  destinationShipId: number;
}

export function emptyContainerTransferredEvent(): onContainerTransferredEvent{
  return { sourceShipId: -1, destinationShipId: -1, selectedContainer: { id: -1, name: '' } }
}
