# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# This stage is used to create the /data folder with the right permissions
WORKDIR /
RUN mkdir -p /data && chown $APP_UID /data
RUN chmod -R 777 /data

# Switch to the app user to avoid running the application as root
USER $APP_UID
WORKDIR /app
EXPOSE 21 
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChangeLetters/ChangeLetters.csproj", "ChangeLetters/"]
COPY ["ChangeLetters.DTOs/ChangeLetters.DTOs.csproj", "ChangeLetters.DTOs/"]
COPY ["ChangeLetters.Client/ChangeLetters.Client.csproj", "ChangeLetters.Client/"]
COPY ["ChangeLetters.SignalR/ChangeLetters.SignalR.csproj", "ChangeLetters.SignalR/"]
RUN dotnet restore "./ChangeLetters/ChangeLetters.csproj"
COPY . .
WORKDIR "/src/ChangeLetters"
RUN dotnet build "./ChangeLetters.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChangeLetters.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChangeLetters.dll"]