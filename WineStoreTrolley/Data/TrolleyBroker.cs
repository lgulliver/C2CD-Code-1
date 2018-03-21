using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace WineStoreTrolley.Data
{
    public class TrolleyBroker
    {
        public TrolleyBroker()
        {
        }



        internal string GetItemsInTrolley(string storageConnectionString, string sessionId)
        {
            CloudStorageAccount storage = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storage.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("trollies");
            table.CreateIfNotExistsAsync().Wait();

            TableOperation retrieveOperation = TableOperation.Retrieve<TrolleyEntity>(sessionId, sessionId);
            var resultTask = table.ExecuteAsync(retrieveOperation);
            resultTask.Wait();

            if(resultTask.Result.Result == null) {
                var entity = new TrolleyEntity(sessionId);
                entity.ContentIds = "";
                entity.ContentItems = "0";

                TableOperation createOperation = TableOperation.InsertOrReplace(entity);
                table.ExecuteAsync(createOperation).Wait();

                return "0;";
            } else {
                var entity = (TrolleyEntity)resultTask.Result.Result;
                return entity.ContentItems + ";" + entity.ContentIds;
            }


        }

        internal int ChangeItemInTrolley(string storageConnectionString, string sessionId, string contentItem)
        {
            CloudStorageAccount storage = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storage.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("trollies");
            table.CreateIfNotExistsAsync().Wait();

            TableOperation retrieveOperation = TableOperation.Retrieve<TrolleyEntity>(sessionId, sessionId);
            var resultTask = table.ExecuteAsync(retrieveOperation);
            resultTask.Wait();

            var trolley = new TrolleyEntity(sessionId);
            if (resultTask.Result.Result != null) {
                trolley = (TrolleyEntity)resultTask.Result.Result;
            }

            if(contentItem.StartsWith("-", StringComparison.InvariantCultureIgnoreCase)) {
                contentItem = contentItem.Substring(1);
                var content = trolley.ContentIds.Split(",");
                var newContent = "";

                bool found = false;
                foreach(var trolleyItem in content) {
                    if (!found)
                    {
                        if (trolleyItem.Equals(contentItem))
                        {
                            found = true;
                            continue;
                        }
                    }

                    newContent = newContent + trolleyItem + ",";
                }

                trolley.ContentIds = newContent;
                trolley.ContentItems = (int.Parse(trolley.ContentItems) - 1).ToString();
            } else {
                trolley.ContentIds = trolley.ContentIds + contentItem + ",";
                trolley.ContentItems = (int.Parse(trolley.ContentItems) + 1).ToString();
            }

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(trolley);
            table.ExecuteAsync(insertOrReplaceOperation).Wait();

            return int.Parse(trolley.ContentItems);
        }

        internal void EmptyTrolley(string storageConnectionString, string sessionId)
        {
            CloudStorageAccount storage = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storage.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("trollies");
            table.CreateIfNotExistsAsync().Wait();

            TableOperation retrieveOperation = TableOperation.Retrieve<TrolleyEntity>(sessionId, sessionId);
            var resultTask = table.ExecuteAsync(retrieveOperation);
            resultTask.Wait();

            var trolley = new TrolleyEntity(sessionId);
            if (resultTask.Result.Result != null)
            {
                trolley = (TrolleyEntity)resultTask.Result.Result;
            }

            trolley.ContentIds = "";
            trolley.ContentItems = "0";

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(trolley);
            table.ExecuteAsync(insertOrReplaceOperation).Wait();
        }

        private class TrolleyEntity : TableEntity
        {

            public TrolleyEntity() { }

            public TrolleyEntity(string sessionId)
            {
                this.RowKey = sessionId;
                this.PartitionKey = sessionId;
                this.ContentItems = "0";
                this.ContentIds = "";
            }

            public string ContentIds { get; set; }
            public string ContentItems { get; set; }
        }

    }
}
