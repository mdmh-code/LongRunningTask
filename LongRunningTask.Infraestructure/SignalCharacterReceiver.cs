using LongRunningTask.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace LongRunningTask.Infraestructure;

public class SignalCharacterReceiver(IHubContext<SignalRCharacterHub> hubContext) : ICharacterReceiver
{
    public Task ReceiveCharacter(string userId, Guid processId, char result, int position, bool isLast)
    {
        return hubContext.Clients.All.SendAsync("ReceiveCharacter", result, userId, processId, position, isLast);
    }
}

