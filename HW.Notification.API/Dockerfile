FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HW.Notification.API/HW.Notification.API.csproj", "HW.Notification.API/"]
COPY ["HW.FileManager.BL/HW.FileManager.BL.csproj", "HW.FileManager.BL/"]
COPY ["HW.Common/HW.Common.csproj", "HW.Common/"]
COPY ["HW.Account.DAL/HW.Account.DAL.csproj", "HW.Account.DAL/"]
COPY ["HW.Account.BLL/HW.Account.BLL.csproj", "HW.Account.BLL/"]
RUN dotnet restore "HW.Notification.API/HW.Notification.API.csproj"
COPY . .
WORKDIR "/src/HW.Notification.API"
RUN dotnet build "HW.Notification.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HW.Notification.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HW.Notification.API.dll"]
