// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;


namespace Microsoft.Solutions.CosmosDB.SQL
{
    public class SQLEntityCollectionBase<TEntity> : IDataRepositoryProvider<TEntity>
        where TEntity : class, IEntityModel<string>
    {
        public IRepository<TEntity, string> EntityCollection { get ;  init ; }

        public SQLEntityCollectionBase(string DataConnectionString, string CollectionName)
        {
            CosmosClient _client = new CosmosClient(DataConnectionString);

            this.EntityCollection = 
                new BusinessTransactionRepository<TEntity, string>(_client,
                CollectionName);
            
        }
    }
}
