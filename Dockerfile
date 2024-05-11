FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MVC_RazorComp_PasswordManager.csproj", "./"]
RUN dotnet restore "MVC_RazorComp_PasswordManager.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "MVC_RazorComp_PasswordManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MVC_RazorComp_PasswordManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MVC_RazorComp_PasswordManager.dll"]
