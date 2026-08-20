# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# Build context: the GoldenCrown folder (the one containing GoldenCrown.slnx and src/).

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["src/GoldenCrown.Domain/GoldenCrown.Domain.csproj", "src/GoldenCrown.Domain/"]
COPY ["src/GoldenCrown.Application/GoldenCrown.Application.csproj", "src/GoldenCrown.Application/"]
COPY ["src/GoldenCrown.Infrastructure/GoldenCrown.Infrastructure.csproj", "src/GoldenCrown.Infrastructure/"]
COPY ["src/GoldenCrown.Api/GoldenCrown.Api.csproj", "src/GoldenCrown.Api/"]
RUN dotnet restore "src/GoldenCrown.Api/GoldenCrown.Api.csproj"
COPY . .
RUN dotnet build "src/GoldenCrown.Api/GoldenCrown.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "src/GoldenCrown.Api/GoldenCrown.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoldenCrown.Api.dll"]
