# this assumes you already built the app as Release configuration
FROM wildernesslabs/m4l-pi as meadow

ARG M4L_USER=m4l

RUN adduser --system --group $M4L_USER

COPY bin/Release/net5.0/App.dll /${M4L_USER}/
COPY bin/Release/net5.0/App.runtimeconfig.json /${M4L_USER}/
COPY bin/Release/net5.0/Bme280.dll /${M4L_USER}/
COPY bin/Release/net5.0/Meadow.Foundation.dll /${M4L_USER}/

USER $M4L_USER

WORKDIR /home/$M4L_USER

ENTRYPOINT ["dotnet", "App.dll"]
