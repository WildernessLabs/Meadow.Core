FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

RUN apt-get update
RUN apt-get upgrade -y

COPY . /src/

#RUN dotnet restore /src/lib/MeadowForLinux.csproj
#RUN dotnet publish -c Release /src/MeadowForLinux.csproj -o /publish --no-restore

RUN dotnet publish -c Release /src/samples/XavierI2C_Sample/XavierI2C_Sample.csproj -o /publish


FROM mcr.microsoft.com/dotnet/runtime:5.0 as runtime

ARG M4L_USER=m4l
ARG HOME_DIR=/home/$M4L_USER

RUN apt-get update
RUN apt-get upgrade -y

RUN apt-get install -y i2c-tools

RUN adduser --system --group $M4L_USER

COPY --from=build /publish/Meadow.Linux.dll ${HOME_DIR}/
COPY --from=build /publish/Meadow.dll ${HOME_DIR}/
COPY --from=build /publish/Meadow.Contracts.dll ${HOME_DIR}/
COPY --from=build /publish/Meadow.Units.dll ${HOME_DIR}/
COPY --from=build /publish/App.dll ${HOME_DIR}/
COPY --from=build /publish/App.runtimeconfig.json ${HOME_DIR}/
COPY --from=build /publish/Meadow.Foundation.dll ${HOME_DIR}/
COPY --from=build /publish/Bno055.dll ${HOME_DIR}/
COPY --from=build /publish/Ccs811.dll ${HOME_DIR}/
COPY --from=build /publish/Si70xx.dll ${HOME_DIR}/

USER $M4L_USER

WORKDIR $HOME_DIR

ENTRYPOINT ["dotnet", "App.dll"]
