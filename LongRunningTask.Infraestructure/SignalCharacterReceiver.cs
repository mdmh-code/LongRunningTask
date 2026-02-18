using LongRunningTask.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace LongRunningTask.Infraestructure;

    public class SignalCharacterReceiver(IHubContext<SignalRCharacterHub> hubContext) : ICharacterReceiver
    {
        public Task ReceiveCharacter(string userId, string processId, char result, int position)
        {
            return hubContext.Clients.All.SendAsync("ReceiveCharacter", result, userId, processId, position);
        }
    }

