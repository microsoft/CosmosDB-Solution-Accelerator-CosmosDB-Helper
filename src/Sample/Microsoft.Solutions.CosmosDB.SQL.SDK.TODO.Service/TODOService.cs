// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Solutions.CosmosDB.SQL.SDK.TODO.Service.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Solutions.CosmosDB.SQL.SDK.TODO.Service
{
    public class TODOService : SQLEntityCollectionBase<ToDo>
    {
        public TODOService(string DataConnectionString, string CollectionName) : base(DataConnectionString, CollectionName)
        {
        }

        public async Task<ToDo> Create(string title, Status status, int percentComplete, DateTime startDate, DateTime endDate, string notes)
        {
            return await this.EntityCollection.AddAsync(
                new ToDo()
                {
                    title = title,
                    status = status,
                    percentComplete = percentComplete,
                    startDate = startDate,
                    endDate = endDate,
                    notes = notes
                }
            );
        }

        public async Task<ToDo> Update(ToDo todo)
        {
            return await this.EntityCollection.SaveAsync(todo);
        }

        public async Task Delete(string id)
        {
            await this.EntityCollection.DeleteAsync(id);
        }

        public async Task<ToDo> Find(string id)
        {
            return await this.EntityCollection.GetAsync(id);
        }

        public async Task<IEnumerable<ToDo>> Search(string notes)
        {
            return await this.EntityCollection.FindAllAsync(new GenericSpecification<ToDo>(x => x.notes.Contains(notes)));
        }
    }
}
