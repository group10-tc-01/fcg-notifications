FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FCG.Notifications.Worker/FCG.Notifications.Worker.csproj", "src/FCG.Notifications.Worker/"]
RUN dotnet restore "./src/FCG.Notifications.Worker/FCG.Notifications.Worker.csproj"
COPY . .
WORKDIR "/src/src/FCG.Notifications.Worker"
RUN dotnet build "./FCG.Notifications.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FCG.Notifications.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG.Notifications.Worker.dll"]