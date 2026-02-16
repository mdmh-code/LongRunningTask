using LongRunningTask.Domain.Interfaces;
using Moq;

namespace LongRunningTask.Domain.Tests
{
    public class CharacterEmmiterTests
    {
        [Test]
        public void EmmitingCharactersShouldCallReceiver()
        {
            // Arrange
            var receiverMock = new Mock<ICharacterReceiver>();
            var delayProviderMock = new Mock<IDelayProvider>();
            var emmiter = new CharacterEmmiter(delayProviderMock.Object, receiverMock.Object);
            
            string input = "Test";
            // Act
            emmiter.EmitCharacters(input);

            // Assert
            receiverMock.Verify(r => r.ReceiveCharacter('T'), Times.Once);
            receiverMock.Verify(r => r.ReceiveCharacter('e'), Times.Once);
            receiverMock.Verify(r => r.ReceiveCharacter('s'), Times.Once);
            receiverMock.Verify(r => r.ReceiveCharacter('t'), Times.Once);
        }
    }
}
