FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/DotNet.Core.Demo/DotNet.Core.Demo.WebAPI/DotNet.Core.Demo.WebAPI.csproj", "src/DotNet.Core.Demo/DotNet.Core.Demo.WebAPI/"]
RUN dotnet restore "src/DotNet.Core.Demo/DotNet.Core.Demo.WebAPI/DotNet.Core.Demo.WebAPI.csproj"
COPY . .
WORKDIR "/src/src/DotNet.Core.Demo/DotNet.Core.Demo.WebAPI"
RUN dotnet build "DotNet.Core.Demo.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DotNet.Core.Demo.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotNet.Core.Demo.WebAPI.dll"]