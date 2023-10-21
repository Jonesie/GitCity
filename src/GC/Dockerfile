FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GC/GC.csproj", "GC/"]
RUN dotnet restore "GC/GC.csproj"
COPY . .
WORKDIR "/src/GC"
RUN dotnet build "GC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GC.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GC.dll"]
