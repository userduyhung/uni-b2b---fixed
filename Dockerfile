# Use the official .NET 8.0 runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 8.0 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/B2BMarketplace.Api/B2BMarketplace.Api.csproj", "src/B2BMarketplace.Api/"]
RUN dotnet restore "src/B2BMarketplace.Api/B2BMarketplace.Api.csproj"

COPY . .
WORKDIR "/src/src/B2BMarketplace.Api"
RUN dotnet build "B2BMarketplace.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "B2BMarketplace.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "B2BMarketplace.Api.dll"]