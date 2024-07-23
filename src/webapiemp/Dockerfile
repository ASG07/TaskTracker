# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["webapiemp.csproj", "."]
RUN dotnet restore "webapiemp.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "webapiemp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "webapiemp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["cert.pfx", "/https/cert.pfx"]
ENTRYPOINT ["dotnet", "webapiemp.dll"]
