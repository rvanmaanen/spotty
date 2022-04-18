FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Spotty.WebApp/Spotty.WebApp.csproj", "Spotty.WebApp/"]
COPY ["Spotty/Spotty.csproj", "Spotty/"]
RUN dotnet restore "Spotty.WebApp/Spotty.WebApp.csproj"
COPY . .
WORKDIR "/src/Spotty.WebApp"
RUN dotnet build "Spotty.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spotty.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spotty.WebApp.dll"]