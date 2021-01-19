// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Solutions.CosmosDB.EFCore
{
    public delegate void EFOnModelCreating(ModelBuilder modelBuilder);

    public class BusinessTransactionRepository<TEntity, TIdentifier> : DbContext,
                                                                       IRepository<TEntity, TIdentifier> where TEntity : class, IEntityModel<string>
    {
        public event EFOnModelCreating OnEFModelCreating;

        public DbSet<TEntity> CurrentEntity { get; set; }
        private string connectionString = "";
        private string databaseName = "";
        public BusinessTransactionRepository(string ConnectionString, string DatabaseName)
        {
            connectionString = ConnectionString;
            databaseName = DatabaseName;

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseCosmos(connectionString, databaseName);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TEntity>().ToContainer(typeof(TEntity).Name + "s");

            modelBuilder.Entity<TEntity>()
                   .HasPartitionKey("__partitionkey")
                   .HasKey(e => e.id);

            OnEFModelCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        public async Task<TEntity> GetAsync(TIdentifier id)
        {
            return await this.FindAsync<TEntity>(id);
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            this.CurrentEntity.Add(entity);
            await this.SaveChangesAsync(true);
            return entity;
        }

        public async Task<TEntity> FindAsync(ISpecification<TEntity> specification)
        {
            return await this.CurrentEntity.FirstOrDefaultAsync<TEntity>(specification.Predicate);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await CurrentEntity.Where<TEntity>(x => true).ToArrayAsync();


        }

        public async Task<IEnumerable<TEntity>> FindAllAsync(ISpecification<TEntity> specification)
        {
            return await CurrentEntity.Where<TEntity>(specification.Predicate).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<TIdentifier> identifiers)
        {
            List<TEntity> results = new List<TEntity>();

            foreach (var i in identifiers)
                results.Add(await CurrentEntity.FindAsync(i));

            return results;
        }

        public async Task<TEntity> SaveAsync(TEntity entity)
        {
            this.Update<TEntity>(entity);
            var affected = await this.SaveChangesAsync();
            return entity;
        }


        public async Task DeleteAsync(TIdentifier EntityId)
        {
            this.Remove<TEntity>(this.Find<TEntity>(EntityId));
            await this.SaveChangesAsync(true);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            this.Remove<TEntity>(entity);
            await this.SaveChangesAsync(true);
        }
    }


}
