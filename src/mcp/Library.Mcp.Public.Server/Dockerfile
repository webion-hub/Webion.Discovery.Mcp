FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/mcp/Library.Mcp.Server/Library.Mcp.Server.csproj", "src/mcp/Library.Mcp.Server/"]
RUN dotnet restore "src/mcp/Library.Mcp.Server/Library.Mcp.Server.csproj"
COPY . .
WORKDIR "/src/src/mcp/Library.Mcp.Server"
RUN dotnet build "./Library.Mcp.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Library.Mcp.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Library.Mcp.Server.dll"]
