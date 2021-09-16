// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Solutions.CosmosDB.Mongo.TODO.Service;
using Microsoft.Solutions.CosmosDB.Mongo.TODO.Service.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Microsoft.Solutions.CosmosDB.Mongo.TODO.Tests
{
    [TestClass()]
    public class TODOServiceTests
    {
        static TODOService todoService;
        static string mongoConnString = "{Put Your ConnectionString}";
        static string objectId;

        [TestInitialize]
        public void InitTest()
        {
            todoService = new TODOService(mongoConnString, "COSMOSDB-MONGO");
        }
        
        
        [TestMethod()]
        public async Task TEST01_CreateTODOTest()
        {
            var result = await todoService.Create("test title", Status.New, 0, DateTime.Today, DateTime.Today.AddDays(7), "bla bla");
            objectId = result.id;
        }

        [TestMethod()]
        public async Task TEST02_UpdateTODOTest()
        {
            var createdTodoObj = await todoService.Find(objectId);
            createdTodoObj.notes = "updated bla bla";

            await todoService.Update(createdTodoObj);

        }

        [TestMethod()]
        public async Task TEST03_SearchTODOTest()
        {
            var todos = await todoService.Search("updated");

            foreach (var todo in todos)
            {
                Console.WriteLine($"{todo.id} - {todo.status} - {todo.notes}");
            }
        }

        [TestMethod()]
        public async Task TEST04_RemoveTODOTest()
        {
            var createdTodoObj = await todoService.Find(objectId);
            Console.WriteLine($"{JsonConvert.SerializeObject(createdTodoObj)} will be removed");
            await todoService.Delete(objectId);
        }

    }
}