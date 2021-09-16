#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Sample/Microsoft.Solutions.CosmosDB.WebHost/Microsoft.Solutions.CosmosDB.TODO.WebHost.csproj", "Sample/Microsoft.Solutions.CosmosDB.WebHost/"]
COPY ["Sample/Microsoft.Solutions.CosmosDB.SQL.SDK.TODO.Service/Microsoft.Solutions.CosmosDB.SQL.TODO.Service.csproj", "Sample/Microsoft.Solutions.CosmosDB.SQL.SDK.TODO.Service/"]
COPY ["libraries/Microsoft.Solutions.CosmosDB/Microsoft.Solutions.CosmosDB.csproj", "libraries/Microsoft.Solutions.CosmosDB/"]
COPY ["libraries/Microsoft.Solutions.CosmosDB.SQL/Microsoft.Solutions.CosmosDB.SQL.csproj", "libraries/Microsoft.Solutions.CosmosDB.SQL/"]
RUN dotnet restore "Sample/Microsoft.Solutions.CosmosDB.WebHost/Microsoft.Solutions.CosmosDB.TODO.WebHost.csproj"
COPY . .
WORKDIR "/src/Sample/Microsoft.Solutions.CosmosDB.WebHost"
RUN dotnet build "Microsoft.Solutions.CosmosDB.TODO.WebHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Microsoft.Solutions.CosmosDB.TODO.WebHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microsoft.Solutions.CosmosDB.TODO.WebHost.dll"]