using AzureFunctions.Common.Models;
using AzureFunctions.Common.Responses;
using AzureFunctions.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureFunctions.Functions.Functions
{
    public static class TodoApi
    {
        [FunctionName(nameof(CreateTodo))]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("Received a new todo.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);
            if (string.IsNullOrWhiteSpace(todo?.TaskDescription))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have a TaskDescription"
                });
            }
            TodoEntity entity = new TodoEntity
            {
                CreatedTime = DateTime.UtcNow,
                ETag = "*",
                IsCompleted = false,
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                TaskDescription = todo.TaskDescription,
            };
            TableOperation addOperation = TableOperation.Insert(entity);
            await todoTable.ExecuteAsync(addOperation);
            string message = "New Todo stored on table";
            log.LogInformation(message);
            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = entity
            });
        }

        [FunctionName(nameof(UpdateTodo))]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Update for todo {id}, received.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);
            TableOperation findOperation = TableOperation.Retrieve<TodoEntity>("TODO", id);
            TableResult findResult = await todoTable.ExecuteAsync(findOperation);
            if (findResult.Result is null)
            {
                return new NotFoundObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Todo not found"
                });
            }
            TodoEntity todoEntity = findResult.Result as TodoEntity;
            todoEntity.IsCompleted = todo.IsCompleted; 
            if (!string.IsNullOrWhiteSpace(todo?.TaskDescription))
            {
                todoEntity.TaskDescription = todo.TaskDescription;
            } 
            
            TableOperation addOperation = TableOperation.Replace(todoEntity);
            await todoTable.ExecuteAsync(addOperation);
            string message = $"Todo: {id}, updated on table";
            log.LogInformation(message);
            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });
        }
    }
}