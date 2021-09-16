// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MongoDB.Driver;
using System;

namespace Microsoft.Solutions.CosmosDB.Mongo
{
    public class MongoEntntyCollectionBase<TEntity> : IDataRepositoryProvider<TEntity>
        where TEntity : class, IEntityModel<string>
    {
        public IRepository<TEntity, string> EntityCollection { get; init; }

        public MongoEntntyCollectionBase(string DataConnectionString, string CollectionName)
        {
            CosmosMongoClientManager.DataconnectionString = DataConnectionString;
            MongoClient _client = CosmosMongoClientManager.Instance;

            this.EntityCollection =
                new BusinessTransactionRepository<TEntity, string>(_client,
                CollectionName);

        }
    }

    public sealed class CosmosMongoClientManager
    {
        private CosmosMongoClientManager() { }

        static CosmosMongoClientManager()
        {
        }

        public static string DataconnectionString;

        private static readonly Lazy<MongoClient> _instance =
            new Lazy<MongoClient>(() => new MongoClient(CosmosMongoClientManager.DataconnectionString));

        public static MongoClient Instance
        {
            get { return _instance.Value; }
        }
    }
}
