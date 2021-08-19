using AzureFunctions.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AzureFunctions.Test.Helpers
{
    public class MockCloudTableTodos : CloudTable
    {
        public MockCloudTableTodos() :
            base(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"))
        {
        }

        public MockCloudTableTodos(Uri tableAddress)
            : base(tableAddress)
        {
        }

        public MockCloudTableTodos(Uri tableAbsoluteUri, StorageCredentials credentials)
            : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableTodos(StorageUri tableAddress, StorageCredentials credentials)
            : base(tableAddress, credentials)
        {
        }

        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = StatusCodes.Status200OK,
                Result = TestFactory.GetTodoEntity()
            });
        }

        public override Task<TableQuerySegment<T>> ExecuteQuerySegmentedAsync<T>(TableQuery<T> query, TableContinuationToken token)
        {
            ConstructorInfo ctor = typeof(TableQuerySegment<T>)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(c => c.GetParameters().Count() == 1);
            TableQuerySegment<T> mockQuerySegment = ctor.Invoke(
                new object[] { new List<T>() }) as TableQuerySegment<T>;
            return Task.FromResult(mockQuerySegment);
        }
    }
}
