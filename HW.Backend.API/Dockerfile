FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HW.Backend.API/HW.Backend.API.csproj", "HW.Backend.API/"]
RUN dotnet restore "HW.Backend.API/HW.Backend.API.csproj"
COPY . .
WORKDIR "/src/HW.Backend.API"
RUN dotnet build "HW.Backend.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HW.Backend.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HW.Backend.API.dll"]
