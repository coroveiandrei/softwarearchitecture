using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using InventoryService.Common;
using InventoryService.Persistance;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Services
{
    public class TableStorageConnection : ITableStorageConnection
    {
        private readonly string _connectionString;

        public TableStorageConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<CloudTable> CreateConnection(string tableName)
        {
            bool ok = CloudStorageAccount.TryParse(_connectionString, out CloudStorageAccount storageAccount);
            var client = storageAccount.CreateCloudTableClient();

            var cloudTable = client.GetTableReference(tableName);
            await cloudTable.CreateIfNotExistsAsync();

            return cloudTable;
        }
    }
}
