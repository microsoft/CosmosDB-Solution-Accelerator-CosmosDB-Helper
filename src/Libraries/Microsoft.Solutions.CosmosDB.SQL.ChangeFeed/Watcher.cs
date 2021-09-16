// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Solutions.CosmosDB.SQL;

namespace Microsoft.Solutions.CosmosDB.SQL.ChangeFeed
{
    public class Watcher<TEntity> : BackgroundService
    {
        readonly string dataConnectionString;
        readonly string sourceDatabaseName;
        readonly string sourceContainerName;
        
        private ChangeFeedProcessor changeFeedProcessor;

        public Watcher(string ConnectionString, string MonitoredDatabaseName, string MonitoredContainerName)
        {
            dataConnectionString = ConnectionString;
            sourceDatabaseName = MonitoredDatabaseName;
            sourceContainerName = MonitoredContainerName;
           
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            ChangeFeedProcessHelper<TEntity> changeFeedWatcher = new ChangeFeedProcessHelper<TEntity>();
            changeFeedProcessor = await changeFeedWatcher.InitChangeFeedProcessorAsync(dataConnectionString, sourceDatabaseName, sourceContainerName, HandleChangesAsync);
            Console.WriteLine("Change feed watcher started");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await changeFeedProcessor.StopAsync();
            Console.WriteLine("Change feed watcher stopped");
        }

        protected async Task HandleChangesAsync(IReadOnlyCollection<TEntity> changes, CancellationToken cancellationToken)
        {
            await OnChangedFeedDataSets(changes, cancellationToken);
            
        }

        protected virtual Task OnChangedFeedDataSets(IReadOnlyCollection<TEntity> changes, CancellationToken cancellationToken) {
            return null;
        }
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
