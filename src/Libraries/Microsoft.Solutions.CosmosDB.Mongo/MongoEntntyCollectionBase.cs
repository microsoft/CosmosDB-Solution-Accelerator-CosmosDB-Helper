// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MongoDB.Driver;

namespace Microsoft.Solutions.CosmosDB.Mongo
{
    public class MongoEntntyCollectionBase<TEntity> : IDataRepositoryProvider<TEntity>
        where TEntity : class, IEntityModel<string>
    {
        public IRepository<TEntity, string> EntityCollection { get; init; }

        public MongoEntntyCollectionBase(string DataConnectionString, string CollectionName)
        {
            this.EntityCollection =
                new BusinessTransactionRepository<TEntity, string>(new MongoClient(DataConnectionString),
                CollectionName);
        }
    }
}
