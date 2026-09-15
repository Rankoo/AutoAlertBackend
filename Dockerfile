# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AutoAlertBackEnd.csproj", "./"]
RUN dotnet restore "AutoAlertBackEnd.csproj"

COPY . .
RUN dotnet publish "AutoAlertBackEnd.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80

COPY --from=build /app/publish .
EXPOSE 80

ENTRYPOINT ["dotnet", "AutoAlertBackEnd.dll"]