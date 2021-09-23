#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BikeRentalAPI/BikeRentalAPI.csproj", "BikeRentalAPI/"]
COPY ["DataAccessLayer/DataAccessLayer.csproj", "DataAccessLayer/"]
COPY ["BusinessAccessLayer/BusinessAccessLayer.csproj", "BusinessAccessLayer/"]
RUN dotnet restore "BikeRentalAPI/BikeRentalAPI.csproj"
COPY . .
WORKDIR "/src/BikeRentalAPI"
RUN dotnet build "BikeRentalAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BikeRentalAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BikeRentalAPI.dll"]