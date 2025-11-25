# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar el archivo de proyecto y restaurar dependencias
COPY ["RecipesAPI.csproj", "./"]
RUN dotnet restore "RecipesAPI.csproj"

# Copiar el resto del código y construir
COPY . .
RUN dotnet build "RecipesAPI.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "RecipesAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Instalar curl para healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copiar los archivos publicados
COPY --from=publish /app/publish .

# Crear directorio para uploads
RUN mkdir -p /app/wwwroot/uploads/recipe && \
    mkdir -p /app/wwwroot/uploads/profile && \
    mkdir -p /app/wwwroot/uploads/step && \
    chmod -R 755 /app/wwwroot

# Exponer el puerto
EXPOSE 5107

# Variable de entorno para ASP.NET Core
ENV ASPNETCORE_URLS=http://+:5107
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "RecipesAPI.dll"]
