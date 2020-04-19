FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
COPY . .
ENTRYPOINT ["dotnet", "DotNet.Core.Demo.WebAPI.dll"]