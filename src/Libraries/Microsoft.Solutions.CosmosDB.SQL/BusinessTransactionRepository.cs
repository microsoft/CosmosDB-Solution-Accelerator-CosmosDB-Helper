// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Solutions.CosmosDB.SQL
{

    public class BusinessTransactionRepository<TEntity, TIdentifier> : IRepository<TEntity, TIdentifier> where TEntity : class, IEntityModel<string>
    {
        private readonly Database _database;
        private readonly Container _container;
        private static bool ensured = false;

        public BusinessTransactionRepository(CosmosClient client, string DatabaseName)
        {
            if (!BusinessTransactionRepository<TEntity, TIdentifier>.ensured)
            {
                _database = client.CreateDatabaseIfNotExistsAsync(DatabaseName).Result;
                _container = _database.CreateContainerIfNotExistsAsync(typeof(TEntity).Name + "s", "/__partitionkey").Result;
                BusinessTransactionRepository<TEntity, TIdentifier>.ensured = true;

            } else
            {
                _database = client.GetDatabase(DatabaseName);
                _container = _database.GetContainer(typeof(TEntity).Name + "s");
            }
        }


        public async Task<TEntity> GetAsync(TIdentifier id)
        {
            var iterator = this._container.GetItemQueryIterator<TEntity>($"select * from c where c.id = '{id.ToString()}'");
            if (iterator.HasMoreResults)
            {
                return (await iterator.ReadNextAsync()).FirstOrDefault<TEntity>();

            }
            else
            {
                return null;
            }
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var result = await this._container.CreateItemAsync<TEntity>(entity);
            return result.Resource;
        }

        public async Task<TEntity> FindAsync(ISpecification<TEntity> specification)
        {
            var iterator = this._container.GetItemLinqQueryable<TEntity>().Where(specification.Predicate).ToFeedIterator();

            if (iterator.HasMoreResults)
            {
                return (await iterator.ReadNextAsync()).FirstOrDefault();

            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var iterator = this._container.GetItemLinqQueryable<TEntity>().ToFeedIterator();

            List<TEntity> results = new List<TEntity>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                foreach (var item in response)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        public async Task<IEnumerable<TEntity>> FindAllAsync(ISpecification<TEntity> specification)
        {
            var iterator = this._container.GetItemLinqQueryable<TEntity>().Where(specification.Predicate).ToFeedIterator();

            List<TEntity> results = new List<TEntity>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                foreach (var item in response)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<TIdentifier> identifiers)
        {
            List<TEntity> results = new List<TEntity>();

            foreach (var id in identifiers)
            {
                var iterator = this._container.GetItemLinqQueryable<TEntity>().Where(x => x.id.Equals(id)).ToFeedIterator();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    foreach (var item in response)
                    {
                        results.Add(item);
                    }
                }
            }

            return results;

        }

        public async Task<TEntity> SaveAsync(TEntity entity)
        {
            var result = await this._container.ReplaceItemAsync<TEntity>(entity, entity.id.ToString());
            return result.Resource;

        }

        public async Task DeleteAsync(TIdentifier EntityId)
        {
            var cosmosEntity = await this.GetAsync(EntityId) as CosmosEntityBase;
            await this._container.DeleteItemAsync<TEntity>(EntityId.ToString(), new PartitionKey(cosmosEntity.__partitionkey) );

        }

        public async Task DeleteAsync(TEntity entity)
        {
            var cosmosEntity = entity as CosmosEntityBase;
            await this._container.DeleteItemAsync<TEntity>(entity.id.ToString(), new PartitionKey(cosmosEntity.__partitionkey));
        }
    }
}
