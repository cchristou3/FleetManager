using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Fleet.Api.Features.Real_time;

public enum ClientMethods
{
    ShipLoaded,
    ShipUnloaded,
    ContainerTransferred
}

public class ShipHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine("ShipHub.OnConnectedAsync");
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        Console.WriteLine("ShipHub.OnDisconnectedAsync");
    }

    public async Task SendMessage(string user, string message)
        => await Clients.Others.SendAsync("ReceiveMessage", user, message);
}