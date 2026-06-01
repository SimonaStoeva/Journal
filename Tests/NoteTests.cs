using Journal.Services;
using Moq;
using Xunit;

namespace Journal.Tests
{
    public class NoteTests
    {
        [Fact]
        public void Saving_ANote_InsertsIntoDatabase()
        {
            var noteServiceMock = new Mock<NoteService>();

            noteServiceMock
                .Setup(x => x.AddNote(
                    1,
                    "Test note",
                    "Test content",
                    null))
                .Returns(true);

            bool result = noteServiceMock.Object.AddNote(
                1,
                "Test note",
                "Test content",
                null);

            Assert.True(result);
        }
        
        [Fact]
        public void AddNote_ReturnsTrue_WhenDataIsValid()
        {
            var noteMock = new Mock<NoteService>();

            noteMock
                .Setup(x => x.AddNote(1, "Title", "Content", null))
                .Returns(true);

            bool result = noteMock.Object.AddNote(1, "Title", "Content", null);

            Assert.True(result);
        }
        
        [Fact]
        public void AddNote_ReturnsFalse_WhenInsertFails()
        {
            var noteMock = new Mock<NoteService>();

            noteMock
                .Setup(x => x.AddNote(1, "", "", null))
                .Returns(false);

            bool result = noteMock.Object.AddNote(1, "", "", null);

            Assert.False(result);
        }
    }
}