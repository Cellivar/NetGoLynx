FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS build
WORKDIR /src
COPY ["NetGoLynx/NetGoLynx.csproj", "NetGoLynx/"]
RUN dotnet restore "NetGoLynx/NetGoLynx.csproj"
COPY . .
WORKDIR "/src/NetGoLynx"
RUN dotnet build "NetGoLynx.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "NetGoLynx.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NetGoLynx.dll"]