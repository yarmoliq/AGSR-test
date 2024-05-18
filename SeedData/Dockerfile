FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SeedData/SeedData.csproj", "SeedData/"]
RUN dotnet restore "SeedData/SeedData.csproj"
COPY . .
WORKDIR "/src/SeedData"
RUN dotnet build "SeedData.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SeedData.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SeedData.dll"]
