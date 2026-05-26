FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar todo el código fuente
COPY . .

# Compilar y publicar el proyecto en la carpeta /app/out
RUN dotnet publish "Claims.Api/Claims.Api.csproj" -c Release -o /app/out

# Construir la imagen final solo con el runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiar los archivos publicados desde la etapa de compilación
COPY --from=build /app/out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Claims.Api.dll"]