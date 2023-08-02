FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HW.FileManager.API/HW.FileManager.API.csproj", "HW.FileManager.API/"]
COPY ["HW.Common/HW.Common.csproj", "HW.Common/"]
COPY ["HW.FileManager.BL/HW.FileManager.BL.csproj", "HW.FileManager.BL/"]
RUN dotnet restore "HW.FileManager.API/HW.FileManager.API.csproj"
COPY . .
WORKDIR "/src/HW.FileManager.API"
RUN dotnet build "HW.FileManager.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HW.FileManager.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HW.FileManager.API.dll"]
