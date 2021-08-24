#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Backend.Api/Backend.Api.csproj", "Backend.Api/"]
COPY ["Backend.Data/Backend.Data.csproj", "Backend.Data/"]
COPY ["Backend.Domain/Backend.Domain.csproj", "Backend.Domain/"]
COPY ["Backend.Exceptions/Backend.Exceptions.csproj", "Backend.Exceptions/"]
COPY ["Backend.Infrastructure/Backend.Infrastructure.csproj", "Backend.Infrastructure/"]
COPY ["Backend.Interfaces/Backend.Interfaces.csproj", "Backend.Interfaces/"]
COPY ["Backend.Models/Backend.Models.csproj", "Backend.Models/"]
COPY ["Backend.Patterns/Backend.Patterns.csproj", "Backend.Patterns/"]
COPY ["Backend.Resources/Backend.Resources.csproj", "Backend.Resources/"]
COPY ["Backend.Setup/Backend.Setup.csproj", "Backend.Setup/"]
RUN dotnet restore "Backend.Api/Backend.Api.csproj"
COPY . .
WORKDIR "/src/Backend.Api"
RUN dotnet build "Backend.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Backend.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backend.Api.dll"]