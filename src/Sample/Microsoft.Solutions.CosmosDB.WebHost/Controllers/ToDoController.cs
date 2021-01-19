// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Solutions.CosmosDB.EFCore.TODO.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Solutions.CosmosDB.TODO.WebHost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToDoController : ControllerBase
    {
        private IDataRepositoryProvider<ToDo> todoRepo;

        public ToDoController(IDataRepositoryProvider<ToDo> repo)
        {
            todoRepo = repo;
        }

        [HttpGet]
        public async Task<IEnumerable<ToDo>> Get()
        {
            return await todoRepo.EntityCollection.GetAllAsync();
        }

        [HttpGet]
        [Route("FindNotes")]
        public async Task<IEnumerable<ToDo>> FindNotes(string searchValue)
        {
            return await todoRepo.EntityCollection.FindAllAsync(new GenericSpecification<ToDo>(x => x.notes.Contains(searchValue)));
        }

        [HttpGet]
        [Route("FindTitle")]
        public async Task<IEnumerable<ToDo>> FindTitle(string searchValue)
        {
            return await todoRepo.EntityCollection.FindAllAsync(new GenericSpecification<ToDo>(x => x.title.Contains(searchValue)));
        }

        [HttpPost]
        public async Task<ToDo> AddNew(ToDo todo)
        {
            return await todoRepo.EntityCollection.AddAsync(
                new ToDo()
                {
                    title = todo.title,
                    startDate = todo.startDate,
                    endDate = todo.endDate,
                    notes = todo.notes,
                    percentComplete = todo.percentComplete,
                    status = todo.status
                }
             );
        }

        [HttpDelete]
        public async Task Delete(string id)
        {
            await todoRepo.EntityCollection.DeleteAsync(id);
        }

        [HttpPut]
        public async Task Update(ToDo todo)
        {
            await todoRepo.EntityCollection.SaveAsync(todo);
        }



    }
}
