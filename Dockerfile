# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore
COPY ["HelloWeb/HelloWeb.csproj", "HelloWeb/"]
RUN dotnet restore "HelloWeb/HelloWeb.csproj"

# Copy source and publish
COPY HelloWeb/ HelloWeb/
WORKDIR "/src/HelloWeb"
RUN dotnet publish "HelloWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HelloWeb.dll"]
