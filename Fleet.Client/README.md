# Fleet.Client

Welcome to Fleet.Client, the Angular-based frontend project for elegantly presenting and managing vehicles in the fleet. This project is built using Angular and Typescript, providing a user-friendly web-based UI for viewing vehicle details and managing their container loads.

## Features

- **User Interface**: Provides a sleek and intuitive web-based interface for users to interact with the fleet's vehicles.
- **Vehicle Details**: Displays comprehensive details of each vehicle in the fleet, including their current container loads.
- **Efficient Management**: Facilitates efficient management of the fleet by allowing users to perform load transfers and other operations seamlessly.

## Getting Started

### Manual Deployment

1. Clone this repository to your local machine.
2. Navigate to the root directory of the Fleet.Client project.
3. Install the necessary dependencies by running: `npm install`
4. Build the project by running: `ng build`
5. Once the build is complete, you can serve the application locally using: `ng serve`
6. The application will be accessible at `http://localhost:4200`.

### Docker Deployment

1. Clone this repository to your local machine.
2. Navigate to the root directory of the Fleet.Client project.
3. Build a Docker image using the provided Dockerfile: `docker build -t fleet-client .`
4. Once the image is built, deploy it to your local Docker environment: `docker run -d -p 8080:80 fleet-client`
5. The application will be accessible at `http://localhost:8080`.

## Contributing

Contributions are welcome! Please see the CONTRIBUTING.md file for guidelines.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Acknowledgments

Thank you for using Fleet.Client! If you have any questions or feedback, please don't hesitate to reach out.

---

*Note: For any additional information or support, refer to the documentation provided within the project.*

