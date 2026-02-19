
namespace LongRunningTask.Domain.Interfaces;

public interface ICharacterReceiver
{
    Task ReceiveCharacter(string userId, Guid processId, char result, int position, bool isLast);
}
