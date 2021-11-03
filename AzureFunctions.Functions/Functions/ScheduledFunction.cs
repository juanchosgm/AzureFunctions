using AzureFunctions.Functions.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.Functions.Functions
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run(
            [TimerTrigger("0 */2 * * * *")] TimerInfo myTimer,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation($"Deleting completed function executed at: {DateTime.Now}");
            string filter = TableQuery.GenerateFilterConditionForBool("IsCompleted", QueryComparisons.Equal, true);
            TableQuery<TodoEntity> query = new TableQuery<TodoEntity>()
                .Where(filter)
                .Take(1);
            TableQuerySegment<TodoEntity> todosCompleted = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            int deleted = 0;
            foreach (TodoEntity todoCompleted in todosCompleted.Results)
            {
                await todoTable.ExecuteAsync(TableOperation.Delete(todoCompleted));
                deleted++;
            }
            log.LogInformation($"Deleted: {deleted} items at: {DateTime.Now}");
            log.LogInformation("HI");
        }
    }
}
