#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

ARG VERSION=0.0.0.0-local
ARG SHIFT_PATH=.

COPY . .

RUN dotnet restore "${SHIFT_PATH}/src/HealthCenterStructureDomainService/HealthCenterStructureDomainService.WebAPI/HealthCenterStructureDomainService.WebAPI.csproj"

COPY . .

WORKDIR "${SHIFT_PATH}/src/HealthCenterStructureDomainService/HealthCenterStructureDomainService.WebAPI"
RUN dotnet build "HealthCenterStructureDomainService.WebAPI.csproj" -c Release -o /app/build /p:Version=$VERSION

FROM build AS publish
ARG VERSION=0.0.0.0-local
RUN dotnet publish "HealthCenterStructureDomainService.WebAPI.csproj" -c Release -o /app/publish /p:Version=$VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.dll"]