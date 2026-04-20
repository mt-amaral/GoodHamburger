FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GoodHamburger/GoodHamburger.csproj", "GoodHamburger/"]
COPY ["GoodHamburger.Shared/GoodHamburger.Shared.csproj", "GoodHamburger.Shared/"]
RUN dotnet restore "./GoodHamburger/GoodHamburger.csproj"
COPY . .
WORKDIR "/src/GoodHamburger"
RUN dotnet publish "./GoodHamburger.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM nginx:1.27-alpine AS final
WORKDIR /usr/share/nginx/html
COPY ["GoodHamburger/nginx.conf", "/etc/nginx/nginx.conf"]
COPY --from=build /app/publish/wwwroot .
EXPOSE 80
