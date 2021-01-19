// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace Microsoft.Solutions.CosmosDB.Mongo
{
    public class BusinessTransactionRepository<TEntity, TIdentifier> : IRepository<TEntity, TIdentifier> where TEntity : class, IEntityModel<string>
    {
        private readonly IMongoDatabase _database;

        public BusinessTransactionRepository(IMongoClient client, string databaseName)
        {
            _database = client.GetDatabase(databaseName);

            if (!BsonClassMap.IsClassMapRegistered(typeof(TEntity)))
                BsonClassMap.RegisterClassMap<TEntity>();
        }

        public async Task<TEntity> GetAsync(TIdentifier id)
        {
            var result = await _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant()).FindAsync<TEntity>(x => x.id.Equals(id));
            return await result.FirstOrDefaultAsync<TEntity>();
        }

        public async Task<TEntity> FindAsync(ISpecification<TEntity> specification)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());
            return await collection.Find(specification.Predicate).FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<TEntity>> FindAllAsync(FilterDefinition<TEntity> builders)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());
            return (await collection.FindAsync(builders)).ToList<TEntity>();
        }



        public async Task<IEnumerable<TEntity>> FindAllAsync(ISpecification<TEntity> specification)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());
            return (await collection.FindAsync(specification.Predicate)).ToList<TEntity>();
        }


        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return (await _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant()).FindAsync(new BsonDocument())).ToList<TEntity>();

        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<TIdentifier> identifiers)
        {

            List<TEntity> results = new List<TEntity>();
            IMongoCollection<TEntity> collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());
            foreach (var i in identifiers)
            {
                results.Add(await this.GetAsync(i));
            }
            return results;
        }

        public async Task<TEntity> SaveAsync(TEntity entity)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());

            await collection.ReplaceOneAsync(x => x.id.Equals(entity.id), entity, new ReplaceOptions
            {
                IsUpsert = true
            });

            return entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());

            await collection.ReplaceOneAsync(x => x.id.Equals(entity.id), entity, new ReplaceOptions
            {
                IsUpsert = true
            });

            return entity;
        }

        public async Task DeleteAsync(TIdentifier entityId)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());

            await collection.DeleteOneAsync(x => x.id.Equals(entityId));
        }

        public async Task DeleteAsync(TEntity entity)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());

            await collection.DeleteOneAsync(x => x.id.Equals(entity.id));
        }

    }
}
