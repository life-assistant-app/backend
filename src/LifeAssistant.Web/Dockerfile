FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/LifeAssistant.Web/LifeAssistant.Web.csproj", "LifeAssistant.Web/"]
RUN dotnet restore "src/LifeAssistant.Web/LifeAssistant.Web.csproj"
COPY . .
WORKDIR "/src/LifeAssistant.Web"
RUN dotnet build "LifeAssistant.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LifeAssistant.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LifeAssistant.Web.dll"]
