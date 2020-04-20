FROM docker.pkg.github.com/louiechannel/dotnet-buildpack/dotnet-buildpack:0.1.0 as build
WORKDIR /tmp
COPY /src ./src
COPY *.sln ./
COPY Nuget.config .
RUN dotnet restore --configfile Nuget.config -p:HideWarningsAndErrors=true -p:EmitAssetsLogMessages=false
RUN dotnet publish src/Ascalon.StreamService -c Release -f netcoreapp3.1 -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /tmp/out ./
ENTRYPOINT [ "./Ascalon.StreamService" ]
