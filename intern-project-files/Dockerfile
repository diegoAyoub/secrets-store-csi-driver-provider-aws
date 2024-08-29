#This was the Dockerfile used to built the image for the .NET application

# Use the official Windows Server Core image as the base for building the .NET application
FROM mcr.microsoft.com/dotnet/sdk:7.0-windowsservercore-ltsc2022 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the project file and restore any dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the entire project and build it
COPY . ./
RUN dotnet build -c Release -o /app/build

# Use the official Windows Server Core image as the base for running the application
FROM mcr.microsoft.com/dotnet/aspnet:7.0-windowsservercore-ltsc2022 AS runtime

# Set the working directory in the container
WORKDIR /app

# Copy the build artifacts from the build environment
COPY --from=build /app/build .

# Specify the entry point for the application
ENTRYPOINT ["dotnet", "mysqlapp.dll"]