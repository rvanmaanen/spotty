FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Spotty.WebApp/Spotty.WebApp.csproj", "Spotty.WebApp/"]
RUN dotnet restore "Spotty.WebApp/Spotty.WebApp.csproj"
COPY . .
WORKDIR "/src/Spotty.WebApp"
RUN dotnet build "Spotty.WebApp.csproj" --os linux --arch x64 --no-restore -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spotty.WebApp.csproj" --os linux --arch x64 --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spotty.WebApp.dll"]