// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using System;
using System.Diagnostics;

namespace Microsoft.Solutions.CosmosDB.SQL
{
    public class SQLEntityCollectionBase<TEntity> : IDataRepositoryProvider<TEntity>
        where TEntity : class, IEntityModel<string>
    {
        public IRepository<TEntity, string> EntityCollection { get ;  init ; }

        public SQLEntityCollectionBase(string DataConnectionString, string CollectionName)
        {
            CosmosClientManager.DataconnectionString = DataConnectionString;
            CosmosClient _client = CosmosClientManager.Instance;

            this.EntityCollection = 
                new BusinessTransactionRepository<TEntity, string>(_client,
                CollectionName);
            
        }
    }

    public sealed class CosmosClientManager
    {
        private CosmosClientManager() { }


        static CosmosClientManager()
        {
            //Type defaultTrace = Type.GetType("Microsoft.Azure.Cosmos.Core.Trace.DefaultTrace,Microsoft.Azure.Cosmos.Direct");
            //TraceSource traceSource = (TraceSource)defaultTrace.GetProperty("TraceSource").GetValue(null);
            //traceSource.Switch.Level = SourceLevels.Off;
            //traceSource.Listeners.Clear();
        }

        public static string DataconnectionString;

        private static readonly Lazy<CosmosClient> _instance =
            new Lazy<CosmosClient>(() => new CosmosClient(CosmosClientManager.DataconnectionString));

        public static CosmosClient Instance
        {
            get { return _instance.Value; }
        }
    }
}
