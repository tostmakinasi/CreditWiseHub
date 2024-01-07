#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CreditWiseHub.API/CreditWiseHub.API.csproj", "CreditWiseHub.API/"]
COPY ["CreditWiseHub.BackgroundJob/CreditWiseHub.BackgroundJob.csproj", "CreditWiseHub.BackgroundJob/"]
COPY ["CreditWiseHub.Service/CreditWiseHub.Service.csproj", "CreditWiseHub.Service/"]
COPY ["CreditWiseHub.Repository/CreditWiseHub.Repository.csproj", "CreditWiseHub.Repository/"]
COPY ["CreditWiseHub.Core/CreditWiseHub.Core.csproj", "CreditWiseHub.Core/"]
RUN dotnet restore "CreditWiseHub.API/CreditWiseHub.API.csproj"
COPY . .
WORKDIR "/src/CreditWiseHub.API"
RUN dotnet build "CreditWiseHub.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CreditWiseHub.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CreditWiseHub.API.dll"]