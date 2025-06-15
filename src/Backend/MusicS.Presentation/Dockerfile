FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Backend/MusicS.Presentation/MusicS.Presentation.csproj", "src/Backend/MusicS.Presentation/"]
COPY ["src/Backend/MusicS.Application/MusicS.Application.csproj", "src/Backend/MusicS.Application/"]
COPY ["src/Backend/MusicS.Infrastructure/MusicS.Infrastructure.csproj", "src/Backend/MusicS.Infrastructure/"]
RUN dotnet restore "src/Backend/MusicS.Presentation/MusicS.Presentation.csproj"
COPY . .
WORKDIR "/src/src/Backend/MusicS.Presentation"
RUN dotnet build "./MusicS.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MusicS.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MusicS.Presentation.dll"]
