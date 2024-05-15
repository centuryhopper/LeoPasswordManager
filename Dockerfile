FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LeoPasswordManager.csproj", "./"]
RUN dotnet restore "LeoPasswordManager.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "LeoPasswordManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LeoPasswordManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LeoPasswordManager.dll"]
