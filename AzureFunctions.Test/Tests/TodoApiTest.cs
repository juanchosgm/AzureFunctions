using AzureFunctions.Common.Models;
using AzureFunctions.Functions.Functions;
using AzureFunctions.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AzureFunctions.Test.Tests
{
    public class TodoApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async Task CreateTodo_Should_Return_200()
        {
            // Arrange
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(
                new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Todo todoRequest = TestFactory.GetTodoRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoRequest);

            // Act
            IActionResult response = await TodoApi.CreateTodo(request, mockTodos, logger);

            // Assert
            OkObjectResult result = response as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async Task UpdateTodo_Should_Return_200()
        {
            // Arrange
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(
                new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Todo todoRequest = TestFactory.GetTodoRequest();
            Guid todoId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId, todoRequest);

            // Act
            IActionResult response = await TodoApi.UpdateTodo(
                request, mockTodos, todoId.ToString(), logger);

            // Assert
            OkObjectResult result = response as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
