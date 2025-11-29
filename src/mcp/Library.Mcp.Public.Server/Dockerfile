FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/mcp/Library.Mcp.Public.Server/Library.Mcp.Public.Server.csproj", "src/mcp/Library.Mcp.Public.Server/"]
COPY ["src/db/Library.Db/Library.Db.csproj", "src/db/Library.Db/"]
RUN dotnet restore "src/mcp/Library.Mcp.Public.Server/Library.Mcp.Public.Server.csproj"
COPY . .
WORKDIR "/src/src/mcp/Library.Mcp.Public.Server"
RUN dotnet build "./Library.Mcp.Public.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Library.Mcp.Public.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Library.Mcp.Public.Server.dll"]
