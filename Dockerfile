FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /src
COPY source/*.sln ./
COPY source/Volo.Controller/*.csproj ./Volo.Controller/
COPY source/Volo.Controller.Shared/*.csproj ./Volo.Controller.Shared/
COPY source/Volo.Controller.Opcua/*.csproj ./Volo.Controller.Opcua/

RUN dotnet restore
COPY source/. .

WORKDIR /src/Volo.Controller
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app

COPY --from=build /src/Volo.Controller/out ./

ENTRYPOINT ["dotnet", "Volo.Controller.dll", "--settings", "Config/appsettings.json"]