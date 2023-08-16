FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HW.Account.API/HW.Account.API.csproj", "HW.Account.API/"]
COPY ["HW.Common/HW.Common.csproj", "HW.Common/"]
COPY ["HW.Account.BLL/HW.Account.BLL.csproj", "HW.Account.BLL/"]
COPY ["HW.Account.DAL/HW.Account.DAL.csproj", "HW.Account.DAL/"]
COPY ["HW.FileManager.BL/HW.FileManager.BL.csproj", "HW.FileManager.BL/"]
RUN dotnet restore "HW.Account.API/HW.Account.API.csproj"
COPY . .
WORKDIR "/src/HW.Account.API"
RUN dotnet build "HW.Account.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HW.Account.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HW.Account.API.dll"]
