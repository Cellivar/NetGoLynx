FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["NetGoLynx.csproj", "."]
RUN cat NetGoLynx.csproj
RUN dotnet restore "NetGoLynx.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "NetGoLynx.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish --no-restore "NetGoLynx.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetGoLynx.dll"]