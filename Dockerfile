# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore
COPY ["MyNetGame/MyNetGame.csproj", "MyNetGame/"]
RUN dotnet restore "MyNetGame/MyNetGame.csproj"

# Copy source and publish
COPY MyNetGame/ MyNetGame/
WORKDIR "/src/MyNetGame"
RUN dotnet publish "MyNetGame.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyNetGame.dll"]
