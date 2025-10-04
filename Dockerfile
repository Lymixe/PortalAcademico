# Fase 1: Compilación
# Usamos la imagen oficial del SDK de .NET 9 para compilar la aplicación.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos los archivos del proyecto y restauramos las dependencias.
COPY ["PortalAcademico.csproj", "."]
RUN dotnet restore "./PortalAcademico.csproj"
COPY . .
WORKDIR "/src/."

# Publicamos la aplicación en modo Release en una carpeta llamada /app/publish.
RUN dotnet publish "PortalAcademico.csproj" -c Release -o /app/publish

# Fase 2: Ejecución
# Usamos una imagen mucho más ligera que solo contiene lo necesario para ejecutar la app.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Exponemos el puerto en el que la aplicación escuchará.
EXPOSE 8080

# El comando final para iniciar la aplicación.
ENTRYPOINT ["dotnet", "PortalAcademico.dll"]