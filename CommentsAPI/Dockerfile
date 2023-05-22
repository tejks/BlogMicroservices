FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN dotnet dev-certs https
WORKDIR /src
COPY ["CommentsAPI/CommentsAPI.csproj", "CommentsAPI/"]
RUN dotnet restore "CommentsAPI/CommentsAPI.csproj"

COPY . .
WORKDIR "/src/CommentsAPI"
RUN dotnet publish "CommentsAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CommentsAPI.dll"]