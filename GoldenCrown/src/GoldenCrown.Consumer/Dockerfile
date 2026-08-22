# Build context: the GoldenCrown folder (the one containing GoldenCrown.slnx and src/).

# Runtime base image: no ASP.NET Core, the consumer is a plain console app
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER app
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["src/GoldenCrown.Contracts/GoldenCrown.Contracts.csproj", "src/GoldenCrown.Contracts/"]
COPY ["src/GoldenCrown.Consumer/GoldenCrown.Consumer.csproj", "src/GoldenCrown.Consumer/"]
RUN dotnet restore "src/GoldenCrown.Consumer/GoldenCrown.Consumer.csproj"
COPY . .
RUN dotnet build "src/GoldenCrown.Consumer/GoldenCrown.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "src/GoldenCrown.Consumer/GoldenCrown.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoldenCrown.Consumer.dll"]
