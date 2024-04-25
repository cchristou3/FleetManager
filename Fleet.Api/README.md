# Fleet.Api

Welcome to Fleet.Api, the backend API for managing a fleet of ships and trucks. This project is built using .NET Core and C#, providing robust functionality for container management, fleet oversight, and more.

## Features

- **Container Management**: Easily create and manage containers, allocating them to ships and trucks within the fleet.
- **Fleet Management**: Register an unlimited number of ships and trucks, and perform load transfers between them for comprehensive fleet management.
- **Custom Capacities**: Define specific capacities for individual ships and trucks to accommodate varying sizes and capabilities.
- **Persistence**: The state of the fleet and container loads is persistently stored to ensure continuity across service restarts.
- **Offloading Restriction**: Prevent unloading of unreachable goods by restricting offloading from trucks to the most recent load.

## Getting Started

### Manual Deployment

1. Clone this repository to your local machine.
2. Open the solution in your preferred IDE (Visual Studio, Visual Studio Code, etc.).
3. Build the solution using: 
   - `dotnet build`
4. And run the Fleet.Api project using: 
   - `dotnet run`
5. The API will be hosted locally on http://localhost:5000 and can be accessed using the specified endpoints.

### Docker Deployment (In-progress)

1. Clone this repository to your local machine.
2. Navigate to the root directory of the Fleet.Api project.
3. Build a Docker image using the provided Dockerfile:
   - `docker build -t fleet-api:v.1.0.0 .`
4. Once the image is built, deploy it to your local Docker environment:
   - `docker run -d --name fleet-api -p 5000:8080 fleet-api:v.1.0.0`
5. The API will be accessible at http://localhost:5000.
6. To demonstrate the GET /Containers API open the following link in your browser http://localhost:5000/api/Containers

## Contributing

Contributions are welcome! Please stick with the conventions used in the project.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Acknowledgments

Thank you for using Fleet.Api! If you have any questions or feedback, please don't hesitate to reach out.

---

*Note: For detailed API documentation, refer to the Swagger documentation exposed by the API.*
