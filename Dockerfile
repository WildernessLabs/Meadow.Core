FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

RUN apt-get update
RUN apt-get upgrade -y

COPY src/lib /src/
RUN dotnet restore /src/MeadowForLinux.csproj
RUN dotnet publish -c Release /src/MeadowForLinux.csproj -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:5.0 as runtime

COPY --from=build /publish/Meadow.Linux.dll /m4l/
COPY --from=build /publish/Meadow.dll /m4l/
COPY --from=build /publish/Meadow.Contracts.dll /m4l/
COPY --from=build /publish/Meadow.Units.dll /m4l/

ENTRYPOINT ["/bin/bash"]
