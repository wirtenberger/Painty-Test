FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PaintyTest.API/PaintyTest.API.csproj", "PaintyTest.API/"]
COPY ["PaintyTest.Data/PaintyTest.Data.csproj", "PaintyTest.Data/"]
RUN dotnet restore "PaintyTest.API/PaintyTest.API.csproj"
COPY . .
WORKDIR "/src/PaintyTest.API"
RUN dotnet build "PaintyTest.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaintyTest.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaintyTest.API.dll"]