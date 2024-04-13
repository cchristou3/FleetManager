using System;
using Fleet.Api.Shared;

namespace Fleet.Api.Errors;

public static class DomainErrors
{
    public static class Container
    {
        public static readonly (ResultCode Code, Error Error) NameCannotBeEmpty =
            (ResultCode.ValidationError, new Error("Name cannot be empty."));

        public static readonly Func<int, (ResultCode Code, Error Error)> TooLong = maximumLength =>
            (ResultCode.ValidationError, new Error($"Name cannot exceed {maximumLength} characters."));

        public static readonly (ResultCode Code, Error Error) NameMustBeUnique =
            (ResultCode.ValidationError, new Error("Name must be unique."));

        public static readonly (ResultCode Code, Error Error) NotFound = (ResultCode.NotFound,
            new Error("Container not found."));

        public static readonly (ResultCode Code, Error Error) NotLoaded =
            (ResultCode.ValidationError, new Error("The specified container has not been loaded."));

        public static readonly (ResultCode Code, Error Error) LoadedInShip =
            (ResultCode.ValidationError,
                new Error("The specified container is loaded in a ship. Please unload it and try again."));

        public static readonly (ResultCode Code, Error Error) LoadedInTruck =
            (ResultCode.ValidationError,
                new Error("The specified container is loaded in a truck. Please unload it and try again."));
    }

    public static class Ship
    {
        public static readonly (ResultCode Code, Error Error) NameCannotBeEmpty =
            (ResultCode.ValidationError, new Error("Name cannot be empty."));

        public static readonly Func<int, (ResultCode Code, Error Error)> TooLong = maximumLength =>
            (ResultCode.ValidationError, new Error($"Name cannot exceed {maximumLength} characters."));

        public static readonly Func<int, (ResultCode Code, Error Error)> CapacityOutOfBounds = maximumCapacity =>
            (ResultCode.ValidationError, new Error($"Capacity must be between 1 and {maximumCapacity}."));

        public static readonly (ResultCode Code, Error Error) NameMustBeUnique =
            (ResultCode.ValidationError, new Error("Name must be unique."));

        public static readonly (ResultCode Code, Error Error) NotFound = (ResultCode.NotFound,
            new Error("Ship not found."));

        public static readonly (ResultCode Code, Error Error) SourceShipNotFound =
            (ResultCode.NotFound, new Error("The Source Ship was not found."));

        public static readonly (ResultCode Code, Error Error) DestinationShipNotFound =
            (ResultCode.NotFound, new Error("The Destination Ship was not found."));

        public static readonly (ResultCode Code, Error Error) IsFull = (ResultCode.ValidationError,
            new Error("Ship is fully loaded."));

        public static readonly (ResultCode Code, Error Error) DestinationShipIsFull = (ResultCode.ValidationError,
            new Error("Destination Ship is fully loaded."));
    }

    public static class ShipContainer
    {
        public static readonly (ResultCode Code, Error Error) LoadedInAnotherShip =
            (ResultCode.ValidationError, new Error("The container is loaded in another ship."));

        public static readonly (ResultCode Code, Error Error) AlreadyInDestination =
            (ResultCode.ValidationError, new Error("The container is already loaded in the specified ship."));

        public static readonly (ResultCode Code, Error Error) NotInSource =
            (ResultCode.ValidationError, new Error("The specified container does not belong to the Source Ship."));

        public static readonly (ResultCode Code, Error Error) DestinationShipCannotBeSameAsSourceShip = (
            ResultCode.ValidationError,
            new Error("Destination Ship cannot be the same with the Source Ship."));
    }

    public static class Truck
    {
        public static readonly (ResultCode Code, Error Error) NameCannotBeEmpty =
            (ResultCode.ValidationError, new Error("Name cannot be empty."));

        public static readonly Func<int, (ResultCode Code, Error Error)> TooLong = maximumLength =>
            (ResultCode.ValidationError, new Error($"Name cannot exceed {maximumLength} characters."));

        public static readonly (ResultCode Code, Error Error) NameMustBeUnique =
            (ResultCode.ValidationError, new Error("Name must be unique."));

        public static readonly (ResultCode Code, Error Error) NotFound = (ResultCode.NotFound,
            new Error("Truck not found."));

        public static readonly (ResultCode Code, Error Error) SourceTruckNotFound =
            (ResultCode.NotFound, new Error("The Source Truck was not found."));

        public static readonly (ResultCode Code, Error Error) DestinationTruckNotFound =
            (ResultCode.NotFound, new Error("The Destination Truck was not found."));

        public static readonly (ResultCode Code, Error Error) IsFull = (ResultCode.ValidationError,
            new Error("Truck is fully loaded."));

        public static readonly (ResultCode Code, Error Error) DestinationTruckIsFull = (ResultCode.ValidationError,
            new Error("Destination Truck is fully loaded."));

        public static readonly Func<int, (ResultCode Code, Error Error)> CapacityOutOfBounds = maximumCapacity =>
            (ResultCode.ValidationError, new Error($"Capacity must be between 1 and {maximumCapacity}."));

        public static readonly (ResultCode Code, Error Error) IsEmpty =
            (ResultCode.ValidationError, new Error("The specified truck is empty."));

        public static readonly (ResultCode Code, Error Error) SourceIsEmpty =
            (ResultCode.ValidationError, new Error("The Source truck is empty."));
    }

    public static class TruckContainer
    {
        public static readonly (ResultCode Code, Error Error) LoadedInAnotherTruck =
            (ResultCode.ValidationError, new Error("The container is already loaded in another truck."));

        public static readonly (ResultCode Code, Error Error) AlreadyInDestination =
            (ResultCode.ValidationError, new Error("The container is already loaded in the specified truck."));

        public static readonly (ResultCode Code, Error Error) NotInSource =
            (ResultCode.ValidationError, new Error("The specified container does not belong to the Source Truck."));

        public static readonly (ResultCode Code, Error Error) NotLatestLoaded =
            (ResultCode.ValidationError,
                new Error(
                    "Container is unreachable, cannot unload it. Unload the latest loaded container and try again."));

        public static readonly (ResultCode Code, Error Error) DestinationTruckCannotBeSameAsSourceTruck = (
            ResultCode.ValidationError,
            new Error("Destination Ship cannot be the same with the Source Ship."));
    }
}