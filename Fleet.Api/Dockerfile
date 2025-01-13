# Use the official .NET Aspnet image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Set the working directory in the container
WORKDIR /app

# Expose port 8080
EXPOSE 8080
EXPOSE 8081

# Use the .NET SDK image for building and publishing the .NET Core API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set build configuration argument (default is Release)
ARG BUILD_CONFIGURATION=Release

# Set working directory inside the container
WORKDIR /src

# Copy project file to container's current directory
COPY ["Fleet.Api.csproj", "./"]

# Run `donet restore` on the project file to acquire all the
# dependencies of the application
RUN dotnet restore "Fleet.Api.csproj"

# Copy source code to container's current directory
COPY . .

# Set working directory inside the container
WORKDIR "/src/"

# Build the project and output to /app/build directory
RUN dotnet build "Fleet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Set publishing stage
FROM build AS publish

# Publish the project to /app/publish directory
RUN dotnet publish "Fleet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

# Set working directory inside the container
WORKDIR /app

# Copy published output from previous stage
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Fleet.Api.dll"]
