#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# certs
RUN dotnet dev-certs https
WORKDIR /src
COPY ["PostsAPI/PostsAPI.csproj", "PostsAPI/"]
RUN dotnet restore "PostsAPI/PostsAPI.csproj"
COPY . .
WORKDIR "/src/PostsAPI"
RUN dotnet build "PostsAPI.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "PostsAPI.csproj" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
# certs
COPY --from=publish /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostsAPI.dll"]