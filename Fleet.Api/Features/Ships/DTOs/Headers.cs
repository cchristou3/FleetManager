using Microsoft.AspNetCore.Mvc;

namespace Fleet.Api.Features.Ships.DTOs;

public class Headers
{
    [FromHeader(Name = "X-Connection-Id")]
    public string ConnectionId { get; set; }
}