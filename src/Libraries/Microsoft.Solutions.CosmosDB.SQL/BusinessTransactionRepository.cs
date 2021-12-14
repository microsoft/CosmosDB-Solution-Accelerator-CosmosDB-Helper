// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Solutions.CosmosDB.SQL
{

    public class BusinessTransactionRepository<TEntity, TIdentifier> : IRepository<TEntity, TIdentifier> where TEntity : class, IEntityModel<string>
    {
        private readonly Database _database;
        private readonly Container _container;
        static bool _checkedDatabase = false;
        static bool _checkedContainer = false;

        public BusinessTransactionRepository(CosmosClient client, string DatabaseName, string containerName = "")
        {
            if (!BusinessTransactionRepository<TEntity, TIdentifier>._checkedDatabase)
            {
                _database = client.CreateDatabaseIfNotExistsAsync(DatabaseName).Result;
                BusinessTransactionRepository<TEntity, TIdentifier>._checkedDatabase = true;
            }
            else
            {
                _database = client.GetDatabase(DatabaseName);
            }



            if (string.IsNullOrEmpty(containerName))
            {
                if (BusinessTransactionRepository<TEntity, TIdentifier>._checkedContainer)
                {
                    _container = _database.GetContainer(typeof(TEntity).Name + "s");
                }
                else
                {
                    _container = _database.CreateContainerIfNotExistsAsync(typeof(TEntity).Name + "s", "/__partitionkey").Result;
                    BusinessTransactionRepository<TEntity, TIdentifier>._checkedContainer = true;
                }
            }
            else
            {
                if (BusinessTransactionRepository<TEntity, TIdentifier>._checkedContainer)
                {
                    _container = _database.GetContainer(containerName);
                }
                else
                {
                    try
                    {
                        //Try to check it is in the database
                        _container = _database.CreateContainerAsync(containerName, "/__partitionkey").Result;
                        BusinessTransactionRepository<TEntity, TIdentifier>._checkedContainer = true;
                    }
                    catch (Exception ex)
                    {
                        var cosmosException = ex.InnerException as CosmosException;

                        if ((cosmosException) != null)
                            if (cosmosException.StatusCode == System.Net.HttpStatusCode.Conflict)
                            {
                                _container = _database.GetContainer(containerName);
                                BusinessTransactionRepository<TEntity, TIdentifier>._checkedContainer = true;
                            }
                    }
                }
            }
        }


        public async Task<TEntity> GetAsync(TIdentifier id)
        {
            var iterator = this._container.GetItemQueryIterator<TEntity>($"select * from c where c.id = '{id.ToString()}'");

            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    return item;
                }
            }

            return null;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var result = await this._container.CreateItemAsync<TEntity>(entity);
            return result.Resource;
        }

        public async Task<TEntity> FindAsync(ISpecification<TEntity> specification)
        {
            var iterator = this._container.GetItemLinqQueryable<TEntity>().Where(specification.Predicate).ToFeedIterator();

            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    return item;
                }
            }

            return null;
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
            GenericSpecification<TEntity> genericSpecification = specification as GenericSpecification<TEntity>;

            FeedIterator<TEntity> iterator = null;

            if (genericSpecification.OrderBy == null)
            {
                iterator = this._container.GetItemLinqQueryable<TEntity>().Where(specification.Predicate).ToFeedIterator();
            }
            else
            {
                if (genericSpecification.Order == Order.Asc)
                {
                    iterator = this._container.GetItemLinqQueryable<TEntity>().Where(specification.Predicate).OrderBy(specification.OrderBy).ToFeedIterator();
                }
                else
                {
                    iterator = this._container.GetItemLinqQueryable<TEntity>().Where(specification.Predicate).OrderByDescending(specification.OrderBy).ToFeedIterator();
                }
            }

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
                var iterator = this._container.GetItemQueryIterator<TEntity>($"select * from c where c.id = '{id.ToString()}'");

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

        public async Task DeleteAsync(TIdentifier EntityId, dynamic partitionKeyValue = null)
        {
            var cosmosEntity = await this.GetAsync(EntityId) as TEntity;

            if (partitionKeyValue == null)
            {
                await this._container.DeleteItemAsync<TEntity>(cosmosEntity.id.ToString(), new PartitionKey(cosmosEntity.__partitionkey));
            }
            else
            {
                await this._container.DeleteItemAsync<TEntity>(cosmosEntity.id.ToString(), new PartitionKey(partitionKeyValue));
            }
        }

        public async Task DeleteAsync(TEntity entity, dynamic partitionKeyValue = null)
        {
            var cosmosEntity = entity as CosmosDBEntityBase;

            if (partitionKeyValue == null)
            {
                await this._container.DeleteItemAsync<TEntity>(entity.id.ToString(), new PartitionKey(cosmosEntity.__partitionkey));
            }
            else
            {
                await this._container.DeleteItemAsync<TEntity>(entity.id.ToString(), new PartitionKey(partitionKeyValue));
            }

        }

        public async Task<TEntity> Find(ISpecification<TEntity> specification)
        {
            var iterator = this._container.GetItemLinqQueryable<TEntity>().Where(specification.Predicate).ToFeedIterator();

            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    return item;
                }
            }

            return null;
        }
    }
}
