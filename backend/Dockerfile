FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Hypesoft.Domain/Hypesoft.Domain.csproj", "src/Hypesoft.Domain/"]
COPY ["src/Hypesoft.Application/Hypesoft.Application.csproj", "src/Hypesoft.Application/"]
COPY ["src/Hypesoft.Infrastructure/Hypesoft.Infrastructure.csproj", "src/Hypesoft.Infrastructure/"]
COPY ["src/Hypesoft.API/Hypesoft.API.csproj", "src/Hypesoft.API/"]
RUN dotnet restore "src/Hypesoft.API/Hypesoft.API.csproj"

COPY . .
WORKDIR /src/src/Hypesoft.API
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Hypesoft.API.dll"]
