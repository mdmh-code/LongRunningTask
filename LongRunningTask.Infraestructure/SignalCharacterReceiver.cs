using LongRunningTask.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace LongRunningTask.Infraestructure
{
    public class SignalCharacterReceiver(IHubContext<SignalRCharacterHub> hubContext) : ICharacterReceiver
    {
        public void ReceiveCharacter(char character)
        {
            hubContext.Clients.All.SendAsync("ReceiveCharacter", character);
        }
    }
}
