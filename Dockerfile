#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/GitLiveServer/GitLiveServer.csproj", "GitLiveServer/"]
RUN dotnet restore "GitLiveServer/GitLiveServer.csproj"
COPY src/ .
WORKDIR "/src/GitLiveServer"
RUN dotnet build "GitLiveServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GitLiveServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV LD_LIBRARY_PATH=/app/runtimes/debian.9-x64/native/
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GitLiveServer.dll"]