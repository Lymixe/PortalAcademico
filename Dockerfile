# Fase 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos los archivos del proyecto y restauramos las dependencias.
COPY ["PortalAcademico.csproj", "."]
RUN dotnet restore "./PortalAcademico.csproj"
COPY . .
WORKDIR "/src/."

# Publicamos la aplicación en modo Release en una carpeta llamada /app/publish.
RUN dotnet publish "PortalAcademico.csproj" -c Release -o /app/publish

# Fase 2: Ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Exponemos el puerto en el que la aplicación escuchará.
EXPOSE 8080

# El comando final para iniciar la aplicación.
ENTRYPOINT ["dotnet", "PortalAcademico.dll"]