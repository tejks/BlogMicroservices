#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# certs
RUN dotnet dev-certs https
WORKDIR /src
COPY ["CommentsAPI/CommentsAPI.csproj", "CommentsAPI/"]
RUN dotnet restore "CommentsAPI/CommentsAPI.csproj"
COPY . .
WORKDIR "/src/CommentsAPI"
RUN dotnet build "CommentsAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CommentsAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
# certs
COPY --from=publish /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommentsAPI.dll"]