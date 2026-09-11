# ---- Steg 1: Build ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Kopiera bara .csproj-filerna först (bättre layer-caching)
COPY src/TaskTracker.Api/TaskTracker.Api.csproj src/TaskTracker.Api/
COPY src/TaskTracker.Domain/TaskTracker.Domain.csproj src/TaskTracker.Domain/
COPY src/TaskTracker.Infrastructure/TaskTracker.Infrastructure.csproj src/TaskTracker.Infrastructure/
COPY src/TaskTracker.Contracts/TaskTracker.Contracts.csproj src/TaskTracker.Contracts/

RUN dotnet restore src/TaskTracker.Api/TaskTracker.Api.csproj

# kopiera resten av källkoden och bygg
COPY src/ src/
RUN dotnet publish src/TaskTracker.Api/TaskTracker.Api.csproj -c Release -o /app/publish

# ---- Steg2: Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskTracker.Api.dll"]