
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY *.sln ./
COPY src/HookCraft.Core/*.csproj ./src/HookCraft.Core/
COPY src/HookCraft.WebApi/*.csproj ./src/HookCraft.WebApi/
COPY tests/HookCraft.Tests/*.csproj ./tests/HookCraft.Tests/
RUN dotnet restore

COPY . ./
RUN dotnet publish src/HookCraft.WebApi/HookCraft.WebApi.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HookCraft.WebApi.dll"]
