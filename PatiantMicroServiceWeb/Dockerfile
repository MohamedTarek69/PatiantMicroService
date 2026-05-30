# See https://aka.ms/customizecontainer to learn how to customize your debug container
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore
COPY ["PatiantMicroServiceWeb/PatiantMicroService.Web.csproj", "PatiantMicroServiceWeb/"]
RUN dotnet restore "PatiantMicroServiceWeb/PatiantMicroService.Web.csproj"

# Copy everything else
COPY . .

# Build project
WORKDIR "/src/PatiantMicroServiceWeb"
RUN dotnet build "PatiantMicroService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PatiantMicroService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "PatiantMicroService.Web.dll"]