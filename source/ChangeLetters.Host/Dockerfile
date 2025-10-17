# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChangeLetters.FTP/ChangeLetters.FTP.csproj", "ChangeLetters.FTP/"]
COPY ["ChangeLetters.Host/ChangeLetters.Host.csproj", "ChangeLetters.Host/"]
COPY ["ChangeLetters.Domain/ChangeLetters.Domain.csproj", "ChangeLetters.Domain/"]
COPY ["ChangeLetters.Models/ChangeLetters.Models.csproj", "ChangeLetters.Models/"]
COPY ["ChangeLetters.Shared/ChangeLetters.Shared.csproj", "ChangeLetters.Shared/"]
COPY ["ChangeLetters.Client/ChangeLetters.Client.csproj", "ChangeLetters.Client/"]
COPY ["ChangeLetters.SignalR/ChangeLetters.SignalR.csproj", "ChangeLetters.SignalR/"]
COPY ["ChangeLetters.Database/ChangeLetters.Database.csproj", "ChangeLetters.Database/"]
COPY ["ChangeLetters.Ai.OpenAi/ChangeLetters.Ai.OpenAi.csproj", "ChangeLetters.Ai.OpenAi/"]
COPY ["ChangeLetters.Application/ChangeLetters.Application.csproj", "ChangeLetters.Application/"]
COPY ["ChangeLetters.Database.SqLite/ChangeLetters.Database.SqLite.csproj", "ChangeLetters.Database.SqLite/"]
COPY ["ChangeLetters.Application.Http/ChangeLetters.Application.Http.csproj", "ChangeLetters.Application.Http/"]

RUN dotnet restore "./ChangeLetters.Host/ChangeLetters.Host.csproj"
COPY . .
WORKDIR "/src/ChangeLetters.Host"
RUN dotnet build "./ChangeLetters.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChangeLetters.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChangeLetters.Host.dll"]