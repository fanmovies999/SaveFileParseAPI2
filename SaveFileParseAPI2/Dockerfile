FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["SaveFileParseAPI2/SaveFileParseAPI2.csproj", "SaveFileParseAPI2/"]
RUN dotnet restore "SaveFileParseAPI2/SaveFileParseAPI2.csproj"
COPY . .
WORKDIR "/src/SaveFileParseAPI2"
RUN dotnet build "SaveFileParseAPI2.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "SaveFileParseAPI2.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "SaveFileParseAPI2.dll"]
