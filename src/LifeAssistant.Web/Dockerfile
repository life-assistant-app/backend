FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . .
RUN dotnet build "src/LifeAssistant.Web/LifeAssistant.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/LifeAssistant.Web/LifeAssistant.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV Logging__Console__FormatterName=Simple
ENTRYPOINT ["dotnet", "LifeAssistant.Web.dll"]
