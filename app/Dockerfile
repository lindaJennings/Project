FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/out/

FROM nginx:alpine
COPY --from=build-env /app/out/wwwroot /usr/share/nginx/html