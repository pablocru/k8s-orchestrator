FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore orchestrator/orchestrator.csproj
RUN dotnet publish orchestrator/orchestrator.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runner
RUN apt-get update && apt-get install -y tzdata
ENV TZ=Europe/Madrid
WORKDIR /app
COPY --from=build /app/publish .
RUN adduser --disabled-password --home /user --gecos '' user
RUN chown -R user /app
RUN chmod -R 755 /app
USER user
EXPOSE 8080
ENTRYPOINT ["dotnet", "orchestrator.dll"]
