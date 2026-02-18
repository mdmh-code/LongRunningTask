
namespace LongRunningTask.Domain.Interfaces
{
    public interface ICharacterReceiver
    {
        Task ReceiveCharacter(string userId, string processId, char result, int position);
    }
}
