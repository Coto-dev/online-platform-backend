﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HW.EmailReciever.MVC/HW.EmailReciever.MVC.csproj", "HW.EmailReciever.MVC/"]
RUN dotnet restore "HW.EmailReciever.MVC/HW.EmailReciever.MVC.csproj"
COPY . .
WORKDIR "/src/HW.EmailReciever.MVC"
RUN dotnet build "HW.EmailReciever.MVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HW.EmailReciever.MVC.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HW.EmailReciever.MVC.dll"]
