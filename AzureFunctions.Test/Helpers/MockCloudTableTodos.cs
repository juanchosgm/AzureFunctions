using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AzureFunctions.Test.Helpers
{
    public class MockCloudTableTodos : CloudTable
    {
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
                HttpStatusCode = (int)HttpStatusCode.OK,
                Result = TestFactory.GetTodoEntity()
            });
        }
    }
}
