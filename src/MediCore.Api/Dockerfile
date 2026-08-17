FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY global.json ./
COPY src/MediCore.Domain/MediCore.Domain.csproj src/MediCore.Domain/
COPY src/MediCore.Application/MediCore.Application.csproj src/MediCore.Application/
COPY src/MediCore.Infrastructure/MediCore.Infrastructure.csproj src/MediCore.Infrastructure/
COPY src/MediCore.Api/MediCore.Api.csproj src/MediCore.Api/
RUN dotnet restore src/MediCore.Api/MediCore.Api.csproj

COPY src/ src/
RUN dotnet publish src/MediCore.Api/MediCore.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MediCore.Api.dll"]
