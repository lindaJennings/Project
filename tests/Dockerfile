FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY app/app.csproj ./app/
COPY tests/tests.csproj ./tests/

RUN dotnet restore ./tests/tests.csproj

COPY app/ ./app
COPY tests/ ./tests

RUN dotnet build ./tests/tests.csproj --configuration Release

CMD ["dotnet", "test", "./tests/tests.csproj", "--no-build", "--verbosity", "normal"]
