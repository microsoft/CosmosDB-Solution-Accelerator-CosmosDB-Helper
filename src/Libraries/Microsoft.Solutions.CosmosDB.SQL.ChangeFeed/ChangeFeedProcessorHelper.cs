// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Solutions.CosmosDB.SQL.ChangeFeed
{
    public class ChangeFeedProcessHelper<TEntity> 
    {
        public async Task<ChangeFeedProcessor> InitChangeFeedProcessorAsync(
                                                                                string DataConnectionString,
                                                                                string SourceDatabaseName,
                                                                                string MonitoredContainerName,
                                                                                Container.ChangesHandler<TEntity> ChangeHandler
                                                                            )
        {
            Container leaseContainer;
            Database database;
            string leaseContainerName = $"{MonitoredContainerName}_lease";
            var processorName = $"{MonitoredContainerName}_changefeedwatcher";
            var instanceName = $"{processorName}_host_{Guid.NewGuid().ToString().Substring(0,8)}";

            //Initialize CosmosDB Connection
            CosmosClient cosmosClient = new CosmosClient(DataConnectionString);

            database = cosmosClient.GetDatabase(SourceDatabaseName);
            if (database is null) throw new NullReferenceException("Specified Database doesn't exist");

            leaseContainer = await database.CreateContainerIfNotExistsAsync(leaseContainerName, "/partitionKey");    
         
            ChangeFeedProcessor changeFeedProcessor = database.GetContainer(MonitoredContainerName)
                .GetChangeFeedProcessorBuilder<TEntity>(processorName: processorName, ChangeHandler)
                    .WithInstanceName(instanceName)
                    .WithLeaseContainer(leaseContainer)
                    .Build();

            Console.WriteLine($"Starting Change Feed Processor...{processorName} in {instanceName}");
            await changeFeedProcessor.StartAsync();
            Console.WriteLine($"Change Feed Processor {processorName} started in {instanceName}");
            return changeFeedProcessor;
        }
    }
}
