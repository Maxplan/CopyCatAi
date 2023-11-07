using CopyCatAiApi.Controllers;
using Xunit;

namespace Tests
{
    public class UserControllerTests
    {
        [Fact]
        public void Test_ReturnsOne()
        {
            // Arrange
            var controller = new UserController();

            // Act
            var result = controller.Test();
        
            // Assert

            Assert.Equal(1, result);
        }
    }
}