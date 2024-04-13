# Fleet Manager

This project is a Fleet Manager application designed to help users efficiently manage their fleet of ships and trucks. It consists of three sub-projects:

1. **Fleet.Api**: The backend API built using .NET Core and C#. It follows the vertical sliced architecture for improved organization and scalability.
2. **Fleet.Api.Testing**: Unit tests for the Fleet.Api backend project to ensure its functionality.
3. **Fleet.Api.Client**: Angular-based frontend to elegantly present the fleet's vehicles and their details through a web-based UI.

## Features

The Fleet.Api backend has been implemented to fulfill the following requirements:

1. **Container Management**: Users are empowered to generate containers and allocate them to their fleet of ships and trucks.
2. **Persistence**: The cargo load of ships is durably stored to ensure continuity of state across service restarts.
3. **Capacity Restriction**: Each ship is constrained by a maximum capacity of 4 containers.
4. **Fleet Management**: Users have the ability to enlist an unlimited count of ships and execute cargo transfers among them, facilitating comprehensive fleet oversight.
5. **Custom Capacities**: Users have the flexibility to define specific capacities for individual ships, accommodating a range of vessel sizes and capabilities.
6. **Truck Management**: Users are equipped to oversee trucks in conjunction with ships, facilitating efficient cargo management for both vehicle types.
7. **Offloading Restriction**:  Unloading from trucks is restricted to the most recent load to prevent the removal of inaccessible goods.

## Project Structure

- **Fleet.Api**:
    - The backend API developed using .NET Core and C# with a vertical sliced architecture.
    - Extensively documented using C#'s XML style comments and conventions.
    - Integrated with Swagger for API documentation.
    - Exposes a Postman collection for easy import and testing.

- **Fleet.Api.Testing**:
    - Contains unit tests to verify the functionality of the Fleet.Api backend.

- **Fleet.Api.Client**:
    - Angular-based frontend providing a user-friendly web-based UI.
    - Displays details of vehicles in the fleet along with their current container loads.
    - Facilitates efficient fleet management.

## Deployment

Both the Fleet.Api and the Fleet.Client have been containerized using Docker, enabling straightforward deployment and scalability.

## Getting Started

1. Clone the repository.
2. Navigate to the respective project directories:
    - `Fleet.Api`
    - `Fleet.Api.Client`
3. Follow the setup instructions in their respective README files to build and run the projects.
4. To run unit tests, right-click on the `Fleet.Api.Testing` project and select 'Run Unit Tests'.

## Contributing

Contributions are welcome! Please adhere to the project's predefined conventions.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Acknowledgments

Special thanks to all contributors and users who have contributed to the improvement of this project.

---

Feel free to reach out with any questions or feedback. Happy coding! 🚀