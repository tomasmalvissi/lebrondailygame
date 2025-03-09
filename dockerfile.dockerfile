 # Usa la imagen base con el SDK de .NET 8 para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia los archivos del proyecto y restaura dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia el resto del código fuente, incluyendo appsettings.json, y compila la aplicación
COPY . ./
RUN dotnet publish -c Release -o /out

# Usa la imagen base con el runtime de .NET 8 para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /out .

# Especificar la configuración de ASP.NET Core (si aplica)
ENV DOTNET_ENVIRONMENT=Production

# Comando para ejecutar la aplicación
CMD ["dotnet", "LBJ.dll"]