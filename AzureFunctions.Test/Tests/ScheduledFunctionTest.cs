using AzureFunctions.Functions.Functions;
using AzureFunctions.Test.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace AzureFunctions.Test.Tests
{
    public class ScheduledFunctionTest
    {
        [Fact]
        public async Task ScheduledFunction_Should_Log_Message()
        {
            // Arrange
            MockCloudTableTodos mockTodos = new MockCloudTableTodos();
            ListLogger logger = TestFactory.CreateLogger(LoggerTypes.List) as ListLogger;

            // Act
            await ScheduledFunction.Run(null, mockTodos, logger);
            string message = logger.Logs[0];

            // Assert
            Assert.Contains("Deleting completed", message);
        }
    }
}
