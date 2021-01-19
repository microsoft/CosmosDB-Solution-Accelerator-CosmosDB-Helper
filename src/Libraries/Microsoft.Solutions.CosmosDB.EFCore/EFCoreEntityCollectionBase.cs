// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.EntityFrameworkCore;

namespace Microsoft.Solutions.CosmosDB.EFCore
{
    public class EFCoreEntityCollectionBase<TEntity> : IDataRepositoryProvider<TEntity>
        where TEntity : class, IEntityModel<string>
    {
        public IRepository<TEntity, string> EntityCollection { get;  init; }
        private static bool ensured = false;


        public EFCoreEntityCollectionBase(string DataConnectionString, string CollectionName)
        {
            var dbContext = new BusinessTransactionRepository<TEntity, string>(DataConnectionString,
                CollectionName);
            
            dbContext.OnEFModelCreating += DbContext_OnEFModelCreating;

            if (!EFCoreEntityCollectionBase<TEntity>.ensured) 
            {
                dbContext.Database.EnsureCreatedAsync().GetAwaiter().GetResult();
                EFCoreEntityCollectionBase<TEntity>.ensured = true;
            } 
            
            this.EntityCollection = dbContext;
        }

        private void DbContext_OnEFModelCreating(ModelBuilder modelBuilder)
        {
            ModelCreating(modelBuilder);
        }

        protected virtual void ModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
