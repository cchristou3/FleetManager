import {HubConnection, HubConnectionBuilder, LogLevel} from '@microsoft/signalr';
import {environment} from "src/environments/environment";

/**
 * Base class for all the services that contain logic related to SignalR's hubs.
 */
export class HubService {
  hubUrl = environment.hubUrl;
  private _hubConnection?: HubConnection;

  public get hubConnection(): HubConnection {
    return this._hubConnection!;
  }

  public get hubConnectionId() {
    return this._hubConnection?.connectionId!;
  }

  /** Creates a connection with the Hub based on the given endpoint.
   *
   * !Any subclasses that wish to override it, require to call it first before any subclass related logic.
   *
   * @param {string} endpoint the endpoint to call (which specifies which Hub to use (PreseneceHub, MessageHub, etc.))
   */
  createHubConnection(endpoint: string) {
    this._hubConnection = new HubConnectionBuilder().withUrl(this.hubUrl + endpoint)
      .configureLogging(LogLevel.Debug)
      .withAutomaticReconnect()
      .build();

    this._hubConnection
      .start()
      .catch(error => console.error(error));
  }

  /** Stops the connection.
   */
  stopHubConnection() {
    console.log('stopHubConnection')
    if (this._hubConnection)
      this._hubConnection
        .stop()
        .catch(error => console.log(error));
  }
}
