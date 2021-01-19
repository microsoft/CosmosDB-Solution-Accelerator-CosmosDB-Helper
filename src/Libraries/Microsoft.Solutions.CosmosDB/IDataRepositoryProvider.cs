// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Solutions.CosmosDB
{
    /// <summary>
    /// Interface for DI in each CosmosDB Helpers
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDataRepositoryProvider<TEntity>
    {
        /// <summary>
        /// Entity Object Collections which has Database CRUD operations
        /// </summary>
         IRepository<TEntity, string> EntityCollection { get;  init ; }
    }
}
