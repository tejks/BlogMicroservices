FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN dotnet dev-certs https
WORKDIR /src
COPY ["PostsAPI/PostsAPI.csproj", "PostsAPI/"]
RUN dotnet restore "PostsAPI/PostsAPI.csproj"

COPY . .
WORKDIR "/src/PostsAPI"
RUN dotnet publish "PostsAPI.csproj" -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PostsAPI.dll"]