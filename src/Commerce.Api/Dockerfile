FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Commerce.Api/Commerce.Api.csproj", "src/Commerce.Api/"]
RUN dotnet restore "src/Commerce.Api/Commerce.Api.csproj"
COPY . .
RUN dotnet publish "src/Commerce.Api/Commerce.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Commerce.Api.dll"]
