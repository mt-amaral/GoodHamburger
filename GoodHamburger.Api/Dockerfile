FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GoodHamburger.Api/GoodHamburger.Api.csproj", "GoodHamburger.Api/"]
COPY ["GoodHamburger.Shared/GoodHamburger.Shared.csproj", "GoodHamburger.Shared/"]
RUN dotnet restore "./GoodHamburger.Api/GoodHamburger.Api.csproj"
COPY . .
WORKDIR "/src/GoodHamburger.Api"
RUN dotnet publish "./GoodHamburger.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GoodHamburger.Api.dll"]
