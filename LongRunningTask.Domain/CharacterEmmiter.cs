using LongRunningTask.Domain.Interfaces;

namespace LongRunningTask.Domain
{

    public class CharacterEmmiter(IDelayProvider delayProvider, ICharacterReceiver receiver)
    {
        /// <summary>
        /// Emits the characters of the given message to all registered receivers.
        /// </summary>
        public void EmitCharacters(string message)
        {
            foreach (var character in message)
            {
                delayProvider.Delay();
                receiver.ReceiveCharacter(character);
            }
        }
    }
}
