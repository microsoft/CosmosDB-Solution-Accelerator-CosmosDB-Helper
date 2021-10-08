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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataConnectionString">Connection String</param>
        /// <param name="CollectionName">Your Database Name</param>
        /// <param name="ContainerName">(Optional) If you don't pass it, The Container will be created by Entity Model Class Name + "s", In Model First Dev, You don't need to use it</param>
        public SQLEntityCollectionBase(string DataConnectionString, string CollectionName, string ContainerName = "")
        {
            CosmosClientManager.DataconnectionString = DataConnectionString;
            CosmosClient _client = CosmosClientManager.Instance;

            this.EntityCollection = 
                new BusinessTransactionRepository<TEntity, string>(_client,
                CollectionName, ContainerName);
            
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
