FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PatientsApi/PatientsApi.csproj", "PatientsApi/"]
RUN dotnet restore "PatientsApi/PatientsApi.csproj"
COPY . .
WORKDIR "/src/PatientsApi"
RUN dotnet build "PatientsApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PatientsApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PatientsApi.dll"]
