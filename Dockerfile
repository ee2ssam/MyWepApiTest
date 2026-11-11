# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore
COPY ["MyFive/MyFive.csproj", "MyFive/"]
RUN dotnet restore "MyFive/MyFive.csproj"

# Copy source and publish
COPY MyFive/ MyFive/
WORKDIR "/src/MyFive"
RUN dotnet publish "MyFive.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyFive.dll"]
