// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Solutions.CosmosDB
{
    /// <summary>
    /// Default CRUD operations in CosmosDB
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TIdentifier"></typeparam>
    public interface IRepository<TEntity, TIdentifier>
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(TIdentifier entityId);
        Task<TEntity> FindAsync(ISpecification<TEntity> specification);
        Task<IEnumerable<TEntity>> FindAllAsync(ISpecification<TEntity> specification);
        Task<TEntity> GetAsync(TIdentifier id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<TIdentifier> identifiers);
        Task<TEntity> SaveAsync(TEntity entity);
    }
}