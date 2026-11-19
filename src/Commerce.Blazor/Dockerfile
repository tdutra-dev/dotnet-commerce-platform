FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Commerce.Blazor/Commerce.Blazor.csproj", "src/Commerce.Blazor/"]
RUN dotnet restore "src/Commerce.Blazor/Commerce.Blazor.csproj"
COPY . .
RUN dotnet publish "src/Commerce.Blazor/Commerce.Blazor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8081
ENTRYPOINT ["dotnet", "Commerce.Blazor.dll"]
