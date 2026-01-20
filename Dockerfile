# Dockerfile para despliegue en Google Cloud Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BibliotecaPersonal/BibliotecaPersonal.csproj", "BibliotecaPersonal/"]
RUN dotnet restore "BibliotecaPersonal/BibliotecaPersonal.csproj"
COPY . .
WORKDIR "/src/BibliotecaPersonal"
RUN dotnet build "BibliotecaPersonal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BibliotecaPersonal.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Cloud Run espera que la app escuche en el puerto definido por PORT
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "BibliotecaPersonal.dll"]
