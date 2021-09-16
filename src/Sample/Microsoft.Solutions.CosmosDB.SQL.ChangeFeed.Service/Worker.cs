using Microsoft.Extensions.Configuration;
using Microsoft.Solutions.CosmosDB.SQL.ChangeFeed;
using Microsoft.Solutions.CosmosDB.SQL.ChangeFeed.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Solutions.CosmosDB.SQL.TODO.ChangeFeed.Service
{
    public class Worker : Watcher<ToDo>
    {
        /// <summary>
        /// Passing configuration by ASPnet core Dependency Injection
        /// This Application sample shows how to detact changes for TODO collection by Microsoft.Solutions.CosmosDB.SQL.TODO.WebHost Demo App
        /// </summary>
        /// <param name="configuration">Check appsettings.json file definition</param>
        public Worker(IConfiguration configuration) : base(configuration["Values:DBConnectionString"], 
                                                            configuration["Values:MonitoredDatabaseName"], 
                                                            configuration["Values:MonitoredContainerName"])
        {
        }

        protected override Task OnChangedFeedDataSets(IReadOnlyCollection<ToDo> changes, CancellationToken cancellationToken)
        {
            //put your business logics with changes
            foreach (var item in changes)
            {
                Console.WriteLine($"Detected operation for item with id {item.id} => {JsonConvert.SerializeObject(item)}");
            }

            return base.OnChangedFeedDataSets(changes, cancellationToken);
        }
    }
}
