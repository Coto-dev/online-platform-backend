FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HW.AdminPanel.API/HW.AdminPanel.API.csproj", "HW.AdminPanel.API/"]
COPY ["HW.AdminPanel.BL/HW.AdminPanel.BL.csproj", "HW.AdminPanel.BL/"]
COPY ["HW.Account.DAL/HW.Account.DAL.csproj", "HW.Account.DAL/"]
COPY ["HW.Common/HW.Common.csproj", "HW.Common/"]
COPY ["HW.Backend.DAL/HW.Backend.DAL.csproj", "HW.Backend.DAL/"]
COPY ["HW.FileManager.BL/HW.FileManager.BL.csproj", "HW.FileManager.BL/"]
RUN dotnet restore "HW.AdminPanel.API/HW.AdminPanel.API.csproj"
COPY . .
WORKDIR "/src/HW.AdminPanel.API"
RUN dotnet build "HW.AdminPanel.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HW.AdminPanel.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HW.AdminPanel.API.dll"]
