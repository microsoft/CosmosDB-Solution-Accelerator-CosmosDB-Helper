// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Microsoft.Solutions.CosmosDB.SQL.SDK.TODO.Service.Models;

namespace Microsoft.Solutions.CosmosDB.SQL.SDK.TODO.Service.Tests
{
    [TestClass()]
    public class TODOServiceTests
    {
        static TODOService todoService;
        static string connString = "{PUT YOUR COSMOS SQL Core API CONNECTION STRING}";
        static string objectId;

        [TestInitialize]
        public void InitTest()
        {
            todoService = new TODOService(connString, "COSMOS-SQLSDK", "CosmosToDo");
        }


        [TestMethod()]
        public async Task TEST01_CreateTODOTest()
        {
            var result = await todoService.Create("test title", Status.New, 0, DateTime.Today, DateTime.Today.AddDays(7), "bla bla");
            objectId = result.id;
            Console.WriteLine(objectId);
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
            Console.WriteLine($"{createdTodoObj} will be removed");
            await todoService.Delete(objectId);
        }
    }
}