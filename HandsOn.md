# How to use CosmosDB Helper Library  

   >You need to have Azure subscription and provisioned Azure Cosmos DB (SQL API or MongoDB API).  
   >
   >Let's start it with Azure Cosmos DB with SQL API first (You may easily shift it for Mongo API later)  
  


## Work with SQL API (Azure Cosmos DB SDK)  

### 0. Pre-requisites   

- We need to prepare Azure CosmosDB resource before starting it.

I assume you have Azure CosmosDB SQL API instance and CosmosDB Mongo API instance both.    
If you don't have it, please prepare your CosmosDB resources. Here is the link [how to provision your Cosmos DB resources from Azure Portal](https://docs.microsoft.com/en-us/azure/cosmos-db/create-cosmosdb-resources-portal)  

Please make sure which API are you working with.  
<img src="./media/azure-cosmos-db-create-new-account-detail.png" width="40%"/>

- .NET Framework 5.0  
  
The Cosmos DB Helper libraries has been compiled with .NET Framework 5.0  
Before starting it, you need to install .NET Framework 5.0 in your development environment.  
If you don't have it, please install it from here - [.NET Framework 5.0 SDK Download](https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-5.0.100-windows-x64-installer)

If you tried to load the solution file with your Visual Studio 2019 without installing .NET Framework 5.0,  
You might get following error :  

>\Program Files\dotnet\sdk\3.1.201\Microsoft.Common.CurrentVersion.targets(1177,5): error MSB3644:   
>The reference assemblies for .NETFramework,Version=v5.0 were not found.   
>To resolve this, install the Developer Pack (SDK/Targeting Pack) for this framework version or retarget your application.  
>You can download .NET Framework Developer Packs at https://aka.ms/msbuild/developerpacks 

Unfortunately this link doesn't point .NET Framework version 5, only 4.8 now.  
To download version 5, you can download it from here [https://dotnet.microsoft.com/download/visual-studio-sdks](https://dotnet.microsoft.com/download/visual-studio-sdks)
and I do recommend you upgrade your Visual Studio version up to 16.8.x.  
  
  
- VS Code  
    
It's OK to start with Visual Studio 2019 but I'm going to start with VS Code in this hands on.    
If you don't have it, you may download it from here - [Download VSCode](https://code.visualstudio.com/) 


### 1. Create Project
Please make sure all of pre-requisites are all set.  
Let's start hands on for now!

Create Console project from Console Window  

```

    dotnet new console -n CosmosHandsOn --framework net5.0

``` 

<img src="./media/dotnet%20new%20console.png" width="70%">

After creating Console project name CosmosHandsOn, Switch the directory to newly created project.  

```

    cd CosmosHandsOn

```

### 2. Add Reference CosmosDB Helper
After switching the directory, add package Reference with Nuget installation

```

    dotnet add package EAE.Solutions.CosmosDB.SQL --version 0.7.3

```

<img src="./media/add%20reference.png" width="90%">  

### 3. Define Entity Class
After referencing CosmosDB Helper, let's define Entity Class will be stored in CosmosDB

Open your VS Code with this command

```

    code .

```

then Add Entity Class with name **ToDo.cs**  

<img src="./media/add%20entity%20class.png" width="60%">  

Define ToDo class with inheriting CosmosDBEntityBase in Microsoft.Solutions.CosmosDB Namespace.  
CosmosDBEntityBase class has responsibility for managing Entity's ID and PartitionKey which may effect to retrieving performance in the cosmos DB.

```csharp

    using System;
    using Microsoft.Solutions.CosmosDB;
    namespace CosmosHandsOn
    {
        public class ToDo : CosmosDBEntityBase
        {
            public string title { get; set; }
            public Status status { get; set; }
            public DateTime startDate { get; set; }
            public DateTime endDate { get; set; }
            public string notes { get; set; }

            private int _percentComplete;

            public int percentComplete
            {
                get { return _percentComplete; }
                set
                {
                    if ((percentComplete < 0) || (percentComplete > 100))
                    {
                        throw new OverflowException("percent value should be between 0 to 100");
                    }
                    else
                    {
                        _percentComplete = percentComplete;
                    }
                }
            }
        }

        public enum Status { New, InProcess, Done }
    }
    
```

### 4. Define Entity Collection Class
Now add new EntityCollectionBase Class with name **ToDoService.cs**  

EntityCollectionBase class is managing Connection Objects and CRUD operations for CosmosDBEntityBase class objects

<img src="./media/add%20todoservice%20class.png" width="60%">    

Define ToDoService class with inheriting SQLEntityCollectionBase which is EntityCollectionBase class for SQL API and specify Entity Class types will manage.  

Once you inherit SQLEntityCollectionBase class, you should generate constructor which has 2 parameters for Database ConnectionString and Database Name.  

The 2 constructor parameters supposed to be passed to SQLEntityCollectionBase class for managing Azure CosmosDB connection and Entity object collection(container).

```csharp

    using System;
    using Microsoft.Solutions.CosmosDB.SQL;

    namespace CosmosHandsOn{
        public class ToDoService : SQLEntityCollectionBase<ToDo>
        {
            public ToDoService(string DataConnectionString, string CollectionName) : base(DataConnectionString, CollectionName)
            {
            }
        }
    }
    
```

<img src="./media/generate%20constructor.png" width="50%" />

SQLEntityCollectionBase class provides features for Azure CosmosDB SQL API CRUD operations internally.  
Let's define CRUD operations in ToDoService.  

SQLEntityCollectionBase Class has EntityCollection property.  
You can operate(Crate, Replace, Update, Delete) Azure CosmosDB with your own Business Entities with it.

Below is the CRUD operations you may easily interact with Azure CosmosDB

- AddAsync method in EntityCollection can insert your object in CosmosDB

```csharp

        public async Task<ToDo> Create(ToDo todo)
        {
            return await this.EntityCollection.AddAsync(todo);
        }
        
```

- SaveAsync method in EntityCollection can update your object in CosmosDB.

```csharp 

        public async Task<ToDo> Update(ToDo todo)
        {
            return await this.EntityCollection.SaveAsync(todo);
        }
        
```

- Get method in EntityCollection can retrieve Entity with Entity's id.(The Entity's identifier which will be managing by CosmosDB Helper class)

```csharp 

        public async Task<ToDo> Find(string id)
        {
            return await this.EntityCollection.GetAsync(id);
        }
        
```

- FindAllAsync method in EntityCollection can retrieve Entities with LINQ statements.

```csharp

        public async Task<IEnumerable<ToDo>> Search(string notes)
        {
            return await this.EntityCollection.FindAllAsync(new GenericSpecification<ToDo>(x => x.notes.Contains(notes)));
        }
        
```

- DeleteAsync method in EntityCollection can delete Entity by Entity's id or Entity itself.

```csharp  

        public async Task Delete(string id)
        {
            await this.EntityCollection.DeleteAsync(id);
        }
        
```

Done! You are ready to create, update, search and delete Entities with these simple methods.

This is full ToDoServices.cs codes

```csharp

    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.Solutions.CosmosDB;
    using Microsoft.Solutions.CosmosDB.SQL;

    namespace CosmosHandsOn{
        public class ToDoService : SQLEntityCollectionBase<ToDo>
        {
            public ToDoService(string DataConnectionString, string CollectionName) : base(DataConnectionString, CollectionName)
            {
                //just passing constructor parameters
            }

            public async Task<ToDo> Create(ToDo todo)
            {
                return await this.EntityCollection.AddAsync(todo);
            }

            public async Task<ToDo> Update(ToDo todo)
            {
                return await this.EntityCollection.SaveAsync(todo);
            }

            public async Task<ToDo> Find(string id)
            {
                return await this.EntityCollection.GetAsync(id);
            }

            public async Task<IEnumerable<ToDo>> Search(string notes)
            {
                return await this.EntityCollection.FindAllAsync(new GenericSpecification<ToDo>(x => x.notes.Contains(notes)));
            }

            public async Task Delete(string id)
            {
                await this.EntityCollection.DeleteAsync(id);
            }  
        }
    }
    
```

Now you may put this ToDoService on wherever you want to host it through MicroService containers or Azure Functions.  

Let's try to test it with Console Application from here.  

Open **Programs.cs** and define CosmosDB ConnectionString.  
To invoking asynchronous methods we need to change **static void Main** method signature to **async static Task Main** .  

Please check your Azure CosmosDB connection string from Azure portal and copy & paste it like below.  

```csharp

    using System;
    using System.Threading.Tasks;

    namespace CosmosHandsOn
    {
        class Program
        {
            static string connectionString = "{Put your connectionstring}";

            async static Task Main(string[] args)
            {
            }
        }
    }
    
```

And instantiate ToDoService with connection string and Database name together.  
The SQLEntityCollectionBase which has been inherited by ToDoService is going to check existence of Database and once it is not exiting, it will generate Database automatically on the fly.

Put this code for instancing ToDoService like below

```csharp

        async static Task Main(string[] args)
        {
            var todoService = new ToDoService(connectionString, "CosmosHandson");
        }
        
```

It will create CosmosHandson Database in your Azure CosmosDB.
Now add the codes for testing ToDoService with this codes

Create ToDo object like below.

```csharp

    
    var todo = new ToDo()
    {
        title = "This is test ToDo",
        startDate = DateTime.Now,
        endDate = DateTime.Now.AddDays(2),
        percentComplete = 0,
        notes = "Cosmos DB is really cool!",
        status = Status.New
    };
    
```

You might be acknowledged that ToDo class has id and __partitionkey properties natively.
You don't necessary to touch them from your code. the id and _partitionkey will be managed by CosmosDB Helper by itself.

After instancing ToDo class let's add the codes following.  

```csharp

    using System;
    using System.Threading.Tasks;

    namespace CosmosHandsOn
    {
        class Program
        {
            static string connectionString = "{Put your connectionstring}";

            async static Task Main(string[] args)
            {
                var todoService = new ToDoService(connectionString, "CosmosHandson");

                var todo = new ToDo()
                {
                    title = "This is test ToDo",
                    startDate = DateTime.Now,
                    endDate = DateTime.Now.AddDays(2),
                    percentComplete = 0,
                    notes = "Cosmos DB is really cool!",
                    status = Status.New
                };

                //Insert ToDO
                var objTodo1 = await todoService.Create(todo);

                //Update ToDo
                objTodo1.title = "Updating test ToDo";
                await todoService.Update(objTodo1);

                //Search ToDo
                var objRetrived = await todoService.Find(objTodo1.id);
                Console.WriteLine($"Found object! it's title is {objRetrived.title}");

                //Find ToDos
                var todos = await todoService.Search("Cosmos");
                foreach (var item in todos)
                {
                    Console.WriteLine($"{item.id} - {item.title} - {item.notes} - {item.status}");
                }

                //Delete ToDos
                await todoService.Delete(objTodo1.id);
                Console.WriteLine($"The Todo object which has title - {objTodo1.title} has been removed");
            }
        }
    }
    
```

and Execute the code with pressing F5(Debugging) or CTRL + F5(Execute without Debugging)  
You may check every CRUD operations are working perfectly.



It's time to use Azure CosmosDB Mongo API Service with our code.  

<br/><br/>  

--- 

<br/><br/>  
## Work with Cosmos DB for Mongo API
>It's easy! We've already implemented all CRUD operations in ToDoService class.   
Azure Cosmos DBHelper provides exactly same development experience with previous one what we did.
>
>To start with Azure CosmosDB for Mongo API, You need to prepare Azure CosmosDB for Mongo API instance.     
If You don't have it yet, deploy Azure CosmosDB for MongoDB like blow before moving further.
>
><img src="./media/provision%20cosmosdb%20mongo.png" width="60%">
>
>After provisioning Azure CosmosDB for Mongo API, grep the connection string.
>The Connection string format for Mongo is different with Azure CosmosDB for SQL API.

### 1. Add Reference  

Try to Add Azure CosmosDB mongo Helper library with nuget package reference in your source directory like below  

``` 

    dotnet add package EAE.Solutions.CosmosDB.Mongo --version 0.7.3

```

<img src="./media/switch%20directory%20add%20package2.png" width="90%">  


### 2. Inherit MongoEntityCollectionBase in ToDoService  
Open the VS Code again from the source folder.  

```

    code .

```

In previous code, ToDoService inherited SQLEntityCollectionBase.  
Just replace ToDoService's base class with **MongoEntityCollectionBase** and replace **using Microsoft.Solutions.CosmosDB.SQL** namespace to **Microsoft.Solutions.CosmosDB.Mongo**.

```csharp
    using System;
    using Microsoft.Solutions.CosmosDB.Mongo;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.Solutions.CosmosDB;

    namespace CosmosHandsOn{
        public class ToDoService : MongoEntntyCollectionBase<ToDo>
        {
            public ToDoService(string DataConnectionString, string CollectionName) : base(DataConnectionString, CollectionName)
            {
            }

            public async Task<ToDo> Create(ToDo todo)
            {
                return await this.EntityCollection.AddAsync(todo);
            }

            public async Task<ToDo> Update(ToDo todo)
            {
                return await this.EntityCollection.SaveAsync(todo);
            }

            public async Task<ToDo> Find(string id)
            {
                return await this.EntityCollection.GetAsync(id);
            }

            public async Task<IEnumerable<ToDo>> Search(string notes)
            {
                return await this.EntityCollection.FindAllAsync(new GenericSpecification<ToDo>(x => x.notes.Contains(notes)));
            }

            public async Task Delete(string id)
            {
                await this.EntityCollection.DeleteAsync(id);
            }
            
        }
    }
```

### 3. Update ConnectionString
> Don't forget to update your connection string for MongoDB in programs.cs file!

That's all we should do to switch our code for using CosmosDB Mongo API.
Just Execute the code with pressing F5(Debugging) or CTRL + F5(Execute without Debugging)


## Dependency Injection in ASP.net

All of CosmosDB Helper is sharing [IDataRepositoryProvider](./src/Libraries/Microsoft.CosmosDB/../Microsoft.Solutions.CosmosDB/IDataRepositoryProvider.cs) interface.  

```csharp

    namespace Microsoft.Solutions.CosmosDB
    {
        public interface IDataRepositoryProvider<TEntity>
        {
            IRepository<TEntity, TIdentifier> EntityCollection { get;  init ; }
        }
    }
    
```

so you may inject your dependency with this interface.

This is the sample code snippet for showing how to inject dependency into DoToService.

```csharp

    services.AddTransient<IDataRepositoryProvider<ToDo>, TODOService>(x => { return new TODOService(configuration["ConnectionString"], "CosmosHandson"); });
    
```

You may check the ToDo ASP.net API sample project from [here](../../tree/main/src/Sample/Microsoft.Solutions.CosmosDB.WebHost)

## Working in Visual Studio 2019 or 2017
Absolutely Visual Studio is the first citizen .NET Development.
If you want to start hands on from .NET Framework, you may reference proper package with Nuget Package manager.

<img src="./media/nuget%20package%20manager.png" width="60%" />


## EAE.Solutions.CosmosDB.SQL.ChangeFeed - Change feed Processor Helper
> Currently it supports **SQL Core API only**, EAE.Solutions.CosmosDB.MongoDB.ChangeFeed will be released next version. 
 

The change feed processor is part of the Azure Cosmos DB SDK V3.  
**Watcher** class in Microsoft.Solutions.CosmosDB.SQL.ChangeFeed namespace derived from BackgroundService, which supports hosting and managing Change feed processors as well as handle change entity collections.

Just Passing monitored entity collections(container) name as parameter to Watcher class then you can catch the changed entity sets.

We can host Watcher on .NET Generic Host in ASP.net core  - https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice 

See the Sample Application for Azure CosmosDB Change feed host [here](./Sample/Microsoft.Solutions.CosmosDB.SQL.ChangeFeed.Service)

```csharp

using Microsoft.Extensions.Configuration;
using Microsoft.Solutions.CosmosDB.SQL.ChangeFeed;
using Microsoft.Solutions.CosmosDB.SQL.ChangeFeed.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Solutions.CosmosDB.SQL.TODO.ChangeFeed.Service
{
    public class Worker : Watcher<ToDo>
    {
        /// <summary>
        /// Passing configuration by ASPnet core Dependency Injection
        /// This Application sample shows how to detact changes for TODO collection by Microsoft.Solutions.CosmosDB.SQL.TODO.WebHost Demo App
        /// </summary>
        /// <param name="configuration">Check appsettings.json file definition</param>
        public Worker(IConfiguration configuration) : base(configuration["Values:DBConnectionString"], 
                                                            configuration["Values:MonitoredDatabaseName"], 
                                                            configuration["Values:MonitoredContainerName"])
        {
        }

        protected override Task OnChangedFeedDataSets(IReadOnlyCollection<ToDo> changes, CancellationToken cancellationToken)
        {
            //put your business logics with changes
            foreach (var item in changes)
            {
                Console.WriteLine($"Detected operation for item with id {item.id} => {JsonConvert.SerializeObject(item)}");
            }

            return base.OnChangedFeedDataSets(changes, cancellationToken);
        }
    }
}


```

Now, Let's make our hands dirt by creating Change Feed Process Host for DoTo Entity Container what we tested in previous handson with Watcher class.

### 1. Create Project
Create Worker Service project from Console Window.  
dotnet CLI tool provides [Worker Service host project template](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new-sdk-templates#web-others).  

Type dotnet CLI command like below in your Console.

```

dotnet new worker -n CosmosHandsOnChangeFeed --framework net5.0

```

<img src="./media/dotnet new worker.png" width="80%" />

After creating project template, move the directory path to CosmosHandsOnChangeFeed

```
cd .\CosmosHandsOnChangeFeed

```
### 2. Add Reference CosmosDB Helper
After switching the directory, add package Reference with Nuget installation.  
This Nuget package contains Watcher class in Microsoft.Solutions.CosmosDB.SQL.ChangeFeed Namespace

```

    dotnet add package EAE.Solutions.CosmosDB.SQL.ChangeFeed --version 0.7.3

```

<img src="./media/add reference changefeed.png" width="90%">  

Then Open VSCode

```

code .

```

You may check the .NET Generic Host ASP.net core project scarffolding source files.

<img src="./media/worker project.png" width="90%">  

### 3. Add ToDo Entity class
Now we are going to add Entity Class where Monitored Container has.  
We are going to watch ToDos Container what we created in previous handson.

Add new file with ToDo.cs and Copy below code and paste to ToDo.cs file

```csharp

using System;
    using Microsoft.Solutions.CosmosDB;
    namespace CosmosHandsOnChangeFeed
    {
        public class ToDo : CosmosDBEntityBase
        {
            public string title { get; set; }
            public Status status { get; set; }
            public DateTime startDate { get; set; }
            public DateTime endDate { get; set; }
            public string notes { get; set; }

            private int _percentComplete;

            public int percentComplete
            {
                get { return _percentComplete; }
                set
                {
                    if ((percentComplete < 0) || (percentComplete > 100))
                    {
                        throw new OverflowException("percent value should be between 0 to 100");
                    }
                    else
                    {
                        _percentComplete = percentComplete;
                    }
                }
            }
        }

        public enum Status { New, InProcess, Done }
    }
    
```
### 4. Inherit Watcher Class in EAE.Solutions.CosmosDB.SQL.ChangeFeed

We have prepared ToDo EntityClass for monitoring entity container.   
Now let's set up Watcher class to minitoring ToDos container.

Open Worker.cs file

``` csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CosmosHandsOnChangeFeed
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
```

There are some sample codes for BackgroundService.  
Add ```Microsoft.Solutions.CosmosDB.SQL.ChangeFeed``` Namespace and then change its Base Class from ```BackgroundService``` to ```Watcher<ToDo>```.  
Then remove whole codes in Worker class like below.  

```csharp

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Solutions.CosmosDB.SQL.ChangeFeed;

namespace CosmosHandsOnChangeFeed
{
    public class Worker : Watcher<ToDo>
    {

     
    }
}

```

Now Add Constructor with IConfiguration Dependency Injection and Pass 3 parameter to its Base class like below.  

the 3 parameters supposed to be passed to Base class is Azure CosmosDB Connectionstring and Monitored Database and Container name.
We are going to update parameter information in appsettings.json file.

ASPnet core has its native Dependency Injection framework and it will read appsettings.json file then pass to every hosted instances at its constructor.

```csharp

    public class Worker : Watcher<ToDo>
    {
        public Worker(IConfiguration configuration ) : base(configuration["Values:DBConnectionString"], 
                                                            configuration["Values:MonitoredDatabaseName"], 
                                                            configuration["Values:MonitoredContainerName"])
        {
        }
    }
    
```
### 5. Override OnChangeFeedDataSets

Now Override ```OnChangeFeedDataSets``` from its Base class like below  

<img src="./media/override OnChangeFeedDataSets.png" width="90%">  

```csharp

        protected override Task OnChangedFeedDataSets(IReadOnlyCollection<ToDo> changes, CancellationToken cancellationToken)
        {
            return base.OnChangedFeedDataSets(changes, cancellationToken);
        }

```

Whenever Monitored Entity Container updated, The Watcher will catch them with ``IReadOnlyCollection<ToDo>`` changes parameter.

Just add the code for Printing out changes into Console window like below:   

```csharp
protected override Task OnChangedFeedDataSets(IReadOnlyCollection<ToDo> changes, CancellationToken cancellationToken)
{
    foreach (var item in changes)
    {
        Console.WriteLine($"Changed ToDo => Title : {item.title} /n Status : {item.status} / Notes : {item.notes} ");
    }
    
    return base.OnChangedFeedDataSets(changes, cancellationToken);
}
```

You can add your own business logics in here instead of Console print out at your requirements.  

Here is the whole Worker.cs codes :

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Solutions.CosmosDB.SQL.ChangeFeed;

namespace CosmosHandsOnChangeFeed
{
    public class Worker : Watcher<ToDo>
    {
        public Worker(IConfiguration configuration ) : base(configuration["Values:DBConnectionString"], 
                                                            configuration["Values:MonitoredDatabaseName"], 
                                                            configuration["Values:MonitoredContainerName"])
        {
        }

        protected override Task OnChangedFeedDataSets(IReadOnlyCollection<ToDo> changes, CancellationToken cancellationToken)
        {
            foreach (var item in changes)
            {
                Console.WriteLine($"Changed ToDo => Title : {item.title} /n Status : {item.status} / Notes : {item.notes} ");
            }
            
            return base.OnChangedFeedDataSets(changes, cancellationToken);
        }

    }
}
```


### 6. Update Configuration for ConnectionString and Monitored Entity Container Information
Open appsettings.json file and add configuration information like below :

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Values" : {
    "DBConnectionString" : "{Put Your Connectionstring}",
    "MonitoredDatabaseName" : "CosmosHandson",
    "MonitoredContainerName" : "ToDos"
    }
}
```

Now you are all set.
Whenerver ToDos entity container has been changed, you can catch them with this Worker class.  
You may put it on Container and scale with Kubernetes clusters.